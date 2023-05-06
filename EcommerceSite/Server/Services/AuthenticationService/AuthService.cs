using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace EcommerceSite.Server.Services.AuthenticationService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(DataContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
        { 
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user == null)
            {
                response.Success = false;
                response.Message = "Не е намерен потребител!";
            }
            else if(!VerifyPasswordHash(password, user.HashPassword, user.SaltPassword))
            {
                response.Success = false;
                response.Message = "Грешна Парола!";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            
            
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            if(await UserExists(user.Email))
            {
                return new ServiceResponse<int> 
                {
                    Success = false,
                    Message = "Потребителя вече съществува!"
                };
            }
            CreateHashPassword(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.HashPassword = passwordHash;
            user.SaltPassword = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<int> { Data = user.Id, Message = "Регистриран успешно!" };
        }

        public async Task<bool> UserExists(string email)
        {
            if(await _context.Users.AnyAsync(user => user.Email.ToLower()
                .Equals(email.ToLower())))
            {
                return true;
            }
            return false;
        }
        public int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        public string GetUserEmail() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] hashPassword, byte[] saltPassword)
        {
            using(var hmac = new HMACSHA512(saltPassword)) 
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(hashPassword);
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user == null)
            {
                return new ServiceResponse<bool> 
                {
                    Success = false,
                    Message = "Потребителя не е намерен!"
                };
            }
            CreateHashPassword(newPassword, out byte[] hashPassword, out byte[] saltPassword);

            user.HashPassword = hashPassword;
            user.SaltPassword = saltPassword;

            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Message = "Паролата беше сменена!"
            };
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }
    }
}
