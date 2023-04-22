namespace EcommerceSite.Server.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartProductResponse>>> GetProductsCart(List<ItemCart> itemCart);
        Task<ServiceResponse<List<CartProductResponse>>> StoreProducts(List<ItemCart> itemCart);
        Task<ServiceResponse<int>> GetItemCartNumber();
        Task<ServiceResponse<List<CartProductResponse>>> GetCartProductsFromDB(int? userId = null);
        Task<ServiceResponse<bool>> AddToCart(ItemCart itemCart);
        Task<ServiceResponse<bool>> UpdateQuantity(ItemCart itemCart);
        Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId);

    }
}
