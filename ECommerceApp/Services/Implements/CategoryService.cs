using ECommerceApp.Commons;
using ECommerceApp.DTOs.CategoryDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Categories;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Caching;
using ECommerceApp.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.Implements
{
    public class CategoryService(
        IUnitOfWork unitOfWork,
        ICategoryMapper mapper,
        IDistributedCache cache,
        IOptions<CacheOptions> cacheOptions,
        ILogger<CategoryService> logger)
        : ICategoryService
    {
        public async Task<ApiResponse<CategoryResponse>> CreateCategoryAsync(CategoryCreateRequest categoryDto)
        {
            try
            {
                if (await unitOfWork.CategoryRepository.ExistsByNameAsync(categoryDto.Name))
                {
                    return new ApiResponse<CategoryResponse>(400, "Category name already exists.");
                }

                var category = mapper.Map(categoryDto);
                category.IsActive = true;

                unitOfWork.CategoryRepository.Add(category);
                await unitOfWork.SaveChangesAsync();
                await RefreshCategoryCacheAsync();
                
                return new ApiResponse<CategoryResponse>(200, mapper.Map(category));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in CategoryService.");
                return new ApiResponse<CategoryResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryResponse>> GetCategoryByIdAsync(int id)
        {
            try
            {
                if (cacheOptions.Value.Enabled)
                {
                    var version = await cache.GetVersionAsync(CatalogCacheKeys.CategoryVersion, logger);
                    var cacheKey = CatalogCacheKeys.CategoryById(id, version);
                    var cachedCategory = await cache.GetJsonAsync<CategoryResponse>(cacheKey, logger);
                    if (cachedCategory != null)
                    {
                        logger.LogInformation("Category {CategoryId} served from cache.", id);
                        return new ApiResponse<CategoryResponse>(200, cachedCategory);
                    }
                }

                var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);

                if (category == null)
                {
                    return new ApiResponse<CategoryResponse>(404, "Category not found.");
                }

                var categoryResponse = mapper.Map(category);
                if (cacheOptions.Value.Enabled)
                {
                    var version = await cache.GetVersionAsync(CatalogCacheKeys.CategoryVersion, logger);
                    var cacheKey = CatalogCacheKeys.CategoryById(id, version);
                    await cache.SetJsonAsync(cacheKey, categoryResponse, GetCatalogCacheDuration(), logger);
                }

                return new ApiResponse<CategoryResponse>(200, categoryResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in CategoryService.");
                return new ApiResponse<CategoryResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateCategoryAsync(int categoryId, CategoryUpdateRequest categoryDto)
        {
            try
            {
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId, true);
                if (category == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Category not found.");
                }

                if (await unitOfWork.CategoryRepository.ExistsByNameAsync(categoryDto.Name, categoryId))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Another category with the same name already exists.");
                }

                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;

                await unitOfWork.SaveChangesAsync();
                await RefreshCategoryCacheAsync();

                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Category with Id {categoryId} updated successfully."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in CategoryService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await unitOfWork.CategoryRepository.GetByIdAsync(id, true);

                if (category == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Category not found.");
                }

                category.IsActive = false;
                await unitOfWork.SaveChangesAsync();
                await RefreshCategoryCacheAsync();

                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Category with Id {id} deleted successfully."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in CategoryService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<CategoryResponse>>> GetAllCategoriesAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var safeRequest = new PaginationRequest
                {
                    PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                    PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
                };

                var categoryQuery = unitOfWork.CategoryRepository.QueryAllActive();
                var totalCount = await categoryQuery.CountAsync();
                
                var categories = await categoryQuery
                    .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                    .Take(safeRequest.PageSize)
                    .ToListAsync();

                var categoryList = new PagedResult<CategoryResponse>(categories.Select(mapper.Map), safeRequest, totalCount);

                return new ApiResponse<PagedResult<CategoryResponse>>(200, categoryList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in CategoryService.");
                return new ApiResponse<PagedResult<CategoryResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        private TimeSpan GetCatalogCacheDuration()
        {
            var expirationMinutes = cacheOptions.Value.CatalogExpirationMinutes > 0
                ? cacheOptions.Value.CatalogExpirationMinutes
                : cacheOptions.Value.DefaultExpirationMinutes;

            return TimeSpan.FromMinutes(expirationMinutes > 0 ? expirationMinutes : 10);
        }

        private Task RefreshCategoryCacheAsync()
        {
            return cacheOptions.Value.Enabled
                ? cache.RefreshVersionAsync(CatalogCacheKeys.CategoryVersion, logger)
                : Task.CompletedTask;
        }
    }
}
