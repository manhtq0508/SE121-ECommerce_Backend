using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> CreateProduct([FromBody] ProductCreateRequest productDto)
        {
            var response = await _productService.CreateProductAsync(productDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProductById(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateProduct/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateProduct(int id, [FromBody] ProductUpdateRequest productDto)
        {
            var response = await _productService.UpdateProductAsync(id, productDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductResponse>>>> GetAllProducts([FromQuery] PaginationRequest paginationRequest)
        {
            var response = await _productService.GetAllProductsAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllProductsByCategory/{categoryId}")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductResponse>>>> GetAllProductsByCategory(int categoryId, [FromQuery] PaginationRequest paginationRequest)
        {
            var response = await _productService.GetAllProductsByCategoryAsync(categoryId, paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateProductStatus/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateProductStatus(int id, [FromBody] ProductStatusUpdateRequest productStatusUpdateDTO)
        {
            var response = await _productService.UpdateProductStatusAsync(id, productStatusUpdateDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
