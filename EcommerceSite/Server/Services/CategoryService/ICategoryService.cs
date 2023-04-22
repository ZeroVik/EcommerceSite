namespace EcommerceSite.Server.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResponse<List<Category>>> GetCategories();
        Task<ServiceResponse<List<Category>>> GetAdminCategories();
        Task<ServiceResponse<List<Category>>> AddCategories(Category category);
        Task<ServiceResponse<List<Category>>> UpdateCategories(Category category);
        Task<ServiceResponse<List<Category>>> DeleteCategories(int id);
    }
}
