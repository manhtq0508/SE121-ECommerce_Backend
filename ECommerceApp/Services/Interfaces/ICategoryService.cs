using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.CategoryDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CategoryCreateRequest categoryDto);
        Task<ApiResponse<CategoryResponse>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<ConfirmationResponse>> UpdateCategoryAsync(int categoryId, CategoryUpdateRequest categoryDto);
        Task<ApiResponse<ConfirmationResponse>> DeleteCategoryAsync(int id);
        Task<ApiResponse<PagedResult<CategoryResponse>>> GetAllCategoriesAsync(PaginationRequest paginationRequest);
    }
}
