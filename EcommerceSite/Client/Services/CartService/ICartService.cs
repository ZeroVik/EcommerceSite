using EcommerceSite.Shared;

namespace EcommerceSite.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCart(ItemCart itemCart);
        Task<List<CartProductResponse>> GetCartProduct();
        Task RemoveProductFromCart(int productId, int productTypeId);
        Task UpdateQuantity(CartProductResponse product);
        Task StoreCart(bool emptyLocalCart);
        Task GetItemCartCount();
    }
}
