using EcommerceSite.Shared;
using System.Security.Claims;

namespace EcommerceSite.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;
        public CartService(DataContext context, IAuthService authService) 
        { 
            _context = context;
            _authService = authService;
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetProductsCart(List<ItemCart> itemCart)
        {
            var result = new ServiceResponse<List<CartProductResponse>>
            {
                Data = new List<CartProductResponse>()
            };
            foreach(var item in itemCart)
            {
                var product = await _context.Products
                    .Where(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();

                if(product == null)
                {
                    continue;
                }

                var productVariant = await _context.ProductVariants
                    .Where(v => v.ProductId == item.ProductId 
                        && v.ProductTypeId == item.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();

                if(productVariant == null)
                {
                    continue;
                }

                var cartProduct = new CartProductResponse
                {
                    ProductId = product.Id,
                    ProductTitle = product.Title,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductType = productVariant.ProductType.Name,
                    ProductTypeId = productVariant.ProductTypeId,
                    Quantity = item.Quantity
                };
                result.Data.Add(cartProduct);
            }
            return result;
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> StoreProducts(List<ItemCart> itemCart)
        {
            itemCart.ForEach(itemCart => itemCart.UserId = _authService.GetUserId());
            _context.ItemsCart.AddRange(itemCart);
            await _context.SaveChangesAsync();

            return await GetCartProductsFromDB();
        }

        public async Task<ServiceResponse<int>> GetItemCartNumber()
        {
            var count = (await _context.ItemsCart.Where(ic => ic.UserId == _authService.GetUserId()).ToListAsync()).Count;
            return new ServiceResponse<int> { Data = count };
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProductsFromDB(int? userId = null)
        {
            if(userId == null)
            {
                userId = _authService.GetUserId();
            }

            return await GetProductsCart(await _context.ItemsCart.Where(ic => ic.UserId == userId).ToListAsync());
        }

        public async Task<ServiceResponse<bool>> AddToCart(ItemCart itemCart)
        {
            itemCart.UserId = _authService.GetUserId();
            var sameItem = await _context.ItemsCart.FirstOrDefaultAsync(ic => ic.ProductId == itemCart.ProductId && ic.ProductTypeId == itemCart.ProductTypeId && ic.UserId == itemCart.UserId);
            if (sameItem == null)
            {
                _context.ItemsCart.Add(itemCart);
            }
            else
            {
                sameItem.Quantity += itemCart.Quantity;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> UpdateQuantity(ItemCart itemCart)
        {
            var itemCartDb = await _context.ItemsCart.FirstOrDefaultAsync(ic => ic.ProductId == itemCart.ProductId && ic.ProductTypeId == itemCart.ProductTypeId && ic.UserId == _authService.GetUserId());
            if (itemCartDb == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Предмета не съществува в количката."
                };
            }
            itemCartDb.Quantity = itemCart.Quantity;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var itemCartDb = await _context.ItemsCart.FirstOrDefaultAsync(ic => ic.ProductId == productId && ic.ProductTypeId == productTypeId && ic.UserId == _authService.GetUserId());
            if (itemCartDb == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Предмета не съществува в количката."
                };
            }
            _context.ItemsCart.Remove(itemCartDb);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }
    }
}
