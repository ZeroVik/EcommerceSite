using EcommerceSite.Shared;

namespace EcommerceSite.Client.Services.ProductService
{
    public interface IProductService
    {
        event Action ProductsChanged;
        List<Product> Products { get; set; }
        List<Product> AdminProducts { get; set; }
        string Message { get; set; }
        int CurrentPage { get; set; }
        int PageCount { get; set; }
        string LastSearchText { get; set; }
        Task GetProduct(string? categoryUrl = null);
        Task<ServiceResponse<Product>> GetProducts(int productId);
        Task SearchProducts(string searchText, int page);
        Task<List<string>> GetSearchSuggestion(string searchText);
        Task GetAdminProducts();
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task DeleteProduct(Product product);

    }
}
