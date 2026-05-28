using ECommerceApp.Commons;
using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Products;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Caching;
using ECommerceApp.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.Implements
{
    public class ProductService(
        IUnitOfWork unitOfWork,
        IProductMapper mapper,
        IDistributedCache cache,
        IOptions<CacheOptions> cacheOptions,
        ILogger<ProductService> logger)
        : IProductService
    {
        public async Task<ApiResponse<ProductResponse>> CreateProductAsync(ProductCreateRequest productDto)
        {
            try
            {
                if (await unitOfWork.ProductRepository.ExistsByNameAsync(productDto.Name))
                {
                    return new ApiResponse<ProductResponse>(400, "Product name already exists.");
                }

                if (!await unitOfWork.CategoryRepository.ExistsByIdAsync(productDto.CategoryId))
                {
                    return new ApiResponse<ProductResponse>(400, "Specified category does not exist.");
                }

                var normalizedImageReference = NormalizeProductImageReference(productDto.ImageUrl);
                if (normalizedImageReference == null)
                {
                    return new ApiResponse<ProductResponse>(400, "Invalid image reference. Use an absolute URL or a product file key returned by /api/Files/get-upload-url.");
                }

                var product = mapper.Map(productDto);
                product.IsAvailable = true;
                product.ImageUrl = normalizedImageReference;

                
                unitOfWork.ProductRepository.Add(product);
                await unitOfWork.SaveChangesAsync();
                await RefreshProductCacheAsync();
                    
                return new ApiResponse<ProductResponse>(200, mapper.Map(product));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in ProductService.");
                return new ApiResponse<ProductResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductResponse>> GetProductByIdAsync(int id)
        {
            try
            {
                if (cacheOptions.Value.Enabled)
                {
                    var version = await cache.GetVersionAsync(CatalogCacheKeys.ProductVersion, logger);
                    var cacheKey = CatalogCacheKeys.ProductById(id, version);
                    var cachedProduct = await cache.GetJsonAsync<ProductResponse>(cacheKey, logger);
                    if (cachedProduct != null)
                    {
                        logger.LogInformation("Product {ProductId} served from cache.", id);
                        return new ApiResponse<ProductResponse>(200, cachedProduct);
                    }
                }

                var product = await unitOfWork.ProductRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return new ApiResponse<ProductResponse>(404, "Product not found.");
                }

                var productResponse = mapper.Map(product);
                if (cacheOptions.Value.Enabled)
                {
                    var version = await cache.GetVersionAsync(CatalogCacheKeys.ProductVersion, logger);
                    var cacheKey = CatalogCacheKeys.ProductById(id, version);
                    await cache.SetJsonAsync(cacheKey, productResponse, GetCatalogCacheDuration(), logger);
                }

                return new ApiResponse<ProductResponse>(200, productResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in ProductService.");
                return new ApiResponse<ProductResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateProductAsync(ProductUpdateRequest productDto)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(productDto.Id, true);
                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                if (await unitOfWork.ProductRepository.ExistsByNameAsync(productDto.Name, productDto.Id))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Another product with the same name already exists.");
                }

                if (!await unitOfWork.CategoryRepository.ExistsByIdAsync(productDto.CategoryId))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Specified category does not exist.");
                }

                var normalizedImageReference = NormalizeProductImageReference(productDto.ImageUrl);
                if (normalizedImageReference == null)
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Invalid image reference. Use an absolute URL or a product file key returned by /api/Files/get-upload-url.");
                }

                product.Name = productDto.Name;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.StockQuantity = productDto.StockQuantity;
                product.ImageUrl = normalizedImageReference;
                product.DiscountPercentage = productDto.DiscountPercentage;
                product.CategoryId = productDto.CategoryId;

                await unitOfWork.SaveChangesAsync();
                await RefreshProductCacheAsync();
                
                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Product with Id {productDto.Id} updated successfully."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in ProductService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdIncludingUnavailableAsync(id, trackChanges: true);

                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                product.IsAvailable = false;

                await unitOfWork.SaveChangesAsync();
                await RefreshProductCacheAsync();
                
                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Product with Id {id} deleted successfully."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in ProductService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<ProductResponse>>> GetAllProductsAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var safeRequest = new PaginationRequest
                {
                    PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                    PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
                };

                var productQuery = unitOfWork.ProductRepository.QueryAllAvailable();
                var totalCount = await productQuery.CountAsync();
                var products = await productQuery
                    .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                    .Take(safeRequest.PageSize)
                    .ToListAsync();

                var productList = new PagedResult<ProductResponse>(products.Select(mapper.Map), safeRequest, totalCount);

                return new ApiResponse<PagedResult<ProductResponse>>(200, productList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in ProductService.");
                return new ApiResponse<PagedResult<ProductResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<ProductResponse>>> GetAllProductsByCategoryAsync(int categoryId, PaginationRequest paginationRequest)
        {
            try
            {
                var safeRequest = new PaginationRequest
                {
                    PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                    PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
                };

                var productQuery = unitOfWork.ProductRepository.QueryByCategory(categoryId);
                var totalCount = await productQuery.CountAsync();

                if (totalCount == 0)
                {
                    return new ApiResponse<PagedResult<ProductResponse>>(404, "Products not found.");
                }

                var products = await productQuery
                    .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                    .Take(safeRequest.PageSize)
                    .ToListAsync();

                var productList = new PagedResult<ProductResponse>(products.Select(mapper.Map), safeRequest, totalCount);

                return new ApiResponse<PagedResult<ProductResponse>>(200, productList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in ProductService.");
                return new ApiResponse<PagedResult<ProductResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateProductStatusAsync(ProductStatusUpdateRequest productStatusUpdateDto)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(productStatusUpdateDto.ProductId, true);

                if (product == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Product not found.");
                }

                product.IsAvailable = productStatusUpdateDto.IsAvailable;

                await unitOfWork.SaveChangesAsync();
                await RefreshProductCacheAsync();
                
                return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse
                {
                    Message = $"Product with Id {productStatusUpdateDto.ProductId} Status Updated successfully."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in ProductService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        private TimeSpan GetCatalogCacheDuration()
        {
            var expirationMinutes = cacheOptions.Value.CatalogExpirationMinutes > 0
                ? cacheOptions.Value.CatalogExpirationMinutes
                : cacheOptions.Value.DefaultExpirationMinutes;

            return TimeSpan.FromMinutes(expirationMinutes > 0 ? expirationMinutes : 10);
        }

        private Task RefreshProductCacheAsync()
        {
            return cacheOptions.Value.Enabled
                ? cache.RefreshVersionAsync(CatalogCacheKeys.ProductVersion, logger)
                : Task.CompletedTask;
        }

        private static string? NormalizeProductImageReference(string? imageReference)
        {
            if (string.IsNullOrWhiteSpace(imageReference))
            {
                return null;
            }

            var normalized = imageReference.Trim().Replace('\\', '/');

            if (Uri.TryCreate(normalized, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return normalized;
            }

            normalized = normalized.TrimStart('/');
            return normalized.StartsWith("products/", StringComparison.OrdinalIgnoreCase) &&
                   normalized.Length > "products/".Length &&
                   !normalized.Contains("..", StringComparison.Ordinal)
                ? normalized
                : null;
        }
    }
}
