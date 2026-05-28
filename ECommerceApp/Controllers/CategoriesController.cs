using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.CategoryDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateCategory")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> CreateCategory([FromBody] CategoryCreateRequest categoryDto)
        {
            var response = await _categoryService.CreateCategoryAsync(categoryDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategoryById(int id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCategory/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateCategory(int id, [FromBody] CategoryUpdateRequest categoryDto)
        {
            var response = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryResponse>>>> GetAllCategories([FromQuery] PaginationRequest paginationRequest)
        {
            var response = await _categoryService.GetAllCategoriesAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
