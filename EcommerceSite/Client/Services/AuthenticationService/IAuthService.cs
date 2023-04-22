namespace EcommerceSite.Client.Services.AuthenticationService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegister request);
        Task<ServiceResponse<string>> Login(UserLogin request);
        Task<ServiceResponse<bool>> ChangePassword(PasswordChange request);
        Task<bool> IsUserAuthenticated();
    }
}
