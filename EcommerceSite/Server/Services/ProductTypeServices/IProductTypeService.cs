namespace EcommerceSite.Server.Services.ProductTypeServices
{
    public interface IProductTypeService
    {
        Task<ServiceResponse<List<ProductType>>> GetProductType();
        Task<ServiceResponse<List<ProductType>>> AddProductType(ProductType productType);
        Task<ServiceResponse<List<ProductType>>> UpdateProductType(ProductType productType);

    }
}
