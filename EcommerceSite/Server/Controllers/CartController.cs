using EcommerceSite.Server.Services.CartService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceSite.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetProductsCart(List<ItemCart> itemsCart)
        {
            var result = await _cartService.GetProductsCart(itemsCart);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreProducts(List<ItemCart> itemsCart)
        {
            var result = await _cartService.StoreProducts(itemsCart);
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<ActionResult<ServiceResponse<bool>>> AddToCart(ItemCart itemCart)
        {
            var result = await _cartService.AddToCart(itemCart);
            return Ok(result);
        }

        [HttpPut("quantity-update")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuantity(ItemCart itemCart)
        {
            var result = await _cartService.UpdateQuantity(itemCart);
            return Ok(result);
        }

        [HttpDelete("{productId}/{productTypeId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var result = await _cartService.RemoveItemFromCart(productId, productTypeId);
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetCartCount()
        {
            return await _cartService.GetItemCartNumber();
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProductsFromDB()
        {
            var result = await _cartService.GetCartProductsFromDB();
            return Ok(result);
        }
    }
}
