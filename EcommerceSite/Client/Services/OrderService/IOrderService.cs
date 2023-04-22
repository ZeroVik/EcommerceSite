namespace EcommerceSite.Client.Services.OrderService
{
    public interface IOrderService
    {
        Task<string> OrderPlace();
        Task<List<OrderViewReview>> GetOrder();
        Task<OrderDetailsResponse> GetOrderDetails(int orderId);
    }
}
