namespace EcommerceSite.Server.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<bool>> OrderPlace(int userId);
        Task<ServiceResponse<List<OrderViewReview>>> GetOrder();
        Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId);
    }
}
