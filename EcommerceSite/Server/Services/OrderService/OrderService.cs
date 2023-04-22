using EcommerceSite.Server.Services.CartService;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceSite.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;
        public OrderService(DataContext context, ICartService cartService, IHttpContextAccessor httpContextAccessor, IAuthService authService)
        {
            _context = context;
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
        }
        

        public async Task<ServiceResponse<bool>> OrderPlace(int userId)
        {
            var products = (await _cartService.GetCartProductsFromDB(userId)).Data;
            decimal totalprice = 0;
            products.ForEach(product => totalprice += product.Price * product.Quantity);

            var itemsOrder = new List<ItemOrder>();
            products.ForEach(product => itemsOrder.Add(new ItemOrder
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                TotalPrice = product.Price * product.Quantity
            }));
            var order = new Order
            {
                UserId = userId,
                CreatedDate = DateTime.Now,
                TotalPrice = totalprice,
                ItemsOrders = itemsOrder
            };
            _context.Orders.Add(order);
            _context.ItemsCart.RemoveRange(_context.ItemsCart.Where(ic => ic.UserId == userId));
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<List<OrderViewReview>>> GetOrder()
        {
            var response = new ServiceResponse<List<OrderViewReview>>();
            var orders = await _context.Orders.Include(o => o.ItemsOrders).ThenInclude(io => io.Product).Where(o => o.UserId == _authService.GetUserId()).OrderByDescending(o => o.CreatedDate).ToListAsync();
            var orderResponse = new List<OrderViewReview>();
            orders.ForEach(o => orderResponse.Add(new OrderViewReview
            {
                Id = o.Id,
                DateOrder = o.CreatedDate,
                TotalPrice = o.TotalPrice,
                Product = o.ItemsOrders.Count > 1 ?
                    $"{o.ItemsOrders.First().Product.Title} и" +
                    $" {o.ItemsOrders.Count - 1} още..." :
                    o.ItemsOrders.First().Product.Title,
                ProductImageUrl = o.ItemsOrders.First().Product.ImageUrl

            }));
            response.Data = orderResponse;
            return response;
        }

        public async Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId)
        {
            var response = new ServiceResponse<OrderDetailsResponse>();
            var order = await _context.Orders.Include(o => o.ItemsOrders)
                .ThenInclude(io => io.Product)
                .Include(o => o.ItemsOrders)
                .ThenInclude(io => io.ProductType)
                .Where(o => o.UserId == _authService.GetUserId() && o.Id == orderId)
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefaultAsync();
            if(order == null)
            {
                response.Success = false;
                response.Message = "Поръчката не е намерена";
                return response;
            }
            var orderDetailsResponse = new OrderDetailsResponse
            {
                CreatedDate= order.CreatedDate,
                TotalPrice= order.TotalPrice,
                Products = new List<OrderDetailsProductResponse>()
            };
            order.ItemsOrders.ForEach(item =>
            orderDetailsResponse.Products.Add(new OrderDetailsProductResponse
            {
                ProductId = item.ProductId,
                ImageUrl = item.Product.ImageUrl,
                ProductType = item.ProductType.Name,
                Quantity = item.Quantity,
                Title = item.Product.Title,
                TotalPrice = item.TotalPrice
            }));
            response.Data = orderDetailsResponse;
            return response;
        }
    }
}
