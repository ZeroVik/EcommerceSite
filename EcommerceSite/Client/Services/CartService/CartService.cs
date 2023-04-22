using Blazored.LocalStorage;
using EcommerceSite.Client.Services.AuthenticationService;
using EcommerceSite.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace EcommerceSite.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private readonly IAuthService _authService;

        public CartService(ILocalStorageService localStorage, HttpClient http, IAuthService authService)
        {
            _localStorage = localStorage;
            _http = http;
            _authService = authService;
        }


        public event Action OnChange;

        public async Task AddToCart(ItemCart itemCart)
        {
            if (await _authService.IsUserAuthenticated())
            {
                await _http.PostAsJsonAsync("api/cart/add", itemCart);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<ItemCart>>("cart");
                if (cart == null)
                {
                    cart = new List<ItemCart>();
                }
                var sameItem = cart.Find(x => x.ProductId == itemCart.ProductId && x.ProductTypeId == itemCart.ProductTypeId);
                if (sameItem == null)
                {
                    cart.Add(itemCart);
                }
                else
                {
                    sameItem.Quantity += itemCart.Quantity;
                }
                await _localStorage.SetItemAsync("cart", cart);
            }
            await GetItemCartCount();

        }

        
        public async Task<List<CartProductResponse>> GetCartProduct()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                var itemCart = await _localStorage.GetItemAsync<List<ItemCart>>("cart");
                if(itemCart == null)
                {
                    return new List<CartProductResponse>();
                }
                var response = await _http.PostAsJsonAsync("api/cart/products", itemCart);
                var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
                return cartProducts.Data;
            }
        }


        public async Task GetItemCartCount()
        {
            if(await _authService.IsUserAuthenticated())
            {
                var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorage.SetItemAsync<int>("itemCartCount", count);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<ItemCart>>("cart");
                await _localStorage.SetItemAsync<int>("itemCartCount", cart != null ? cart.Count : 0);
            }
            OnChange.Invoke();
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            if(await _authService.IsUserAuthenticated())
            {
                await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<ItemCart>>("cart");
                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    await _localStorage.SetItemAsync("cart", cart);
                    await GetItemCartCount();

                }
            }
           
            
        }

        public async Task StoreCart(bool emptyLocalCart = false)
        {
            var localCart = await _localStorage.GetItemAsync<List<ItemCart>>("cart");
            if(localCart == null)
            {
                return;
            }
            await _http.PostAsJsonAsync("api/cart", localCart);
            if(emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            if(await _authService.IsUserAuthenticated())
            {
                var request = new ItemCart
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };
                await _http.PutAsJsonAsync("api/cart/quantity-update", request);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<ItemCart>>("cart");
                if (cart == null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);
                if (cartItem != null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorage.SetItemAsync("cart", cart);
                }
            }
            
        }
    }
}
