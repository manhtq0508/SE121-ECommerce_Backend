using ECommerceApp.Commons;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.DTOs.FeedbackDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController(IFeedbackService feedbackService) : ControllerBase
    {
        // Submits feedback for a product.
        [Authorize]
        [HttpPost("SubmitFeedback")]
        public async Task<ActionResult<ApiResponse<FeedbackResponse>>> SubmitFeedback([FromBody] FeedbackCreateRequest feedbackCreateDto)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != feedbackCreateDto.CustomerId)
            {
                return Forbid();
            }

            var response = await feedbackService.SubmitFeedbackAsync(feedbackCreateDto);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all feedback for a specific product.
        [HttpGet("GetFeedbackForProduct/{productId}")]
        public async Task<ActionResult<ApiResponse<ProductFeedbackResponse>>> GetFeedbackForProduct(int productId, [FromQuery] PaginationRequest paginationRequest)
        {
            var response = await feedbackService.GetFeedbackForProductAsync(productId, paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all feedback (Admin use).
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllFeedback")]
        public async Task<ActionResult<ApiResponse<PagedResult<FeedbackResponse>>>> GetAllFeedback([FromQuery] PaginationRequest paginationRequest)
        {
            var response = await feedbackService.GetAllFeedbackAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates a specific feedback entry.
        [Authorize]
        [HttpPut("UpdateFeedback")]
        public async Task<ActionResult<ApiResponse<FeedbackResponse>>> UpdateFeedback([FromBody] FeedbackUpdateRequest feedbackUpdateDto)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != feedbackUpdateDto.CustomerId)
            {
                return Forbid();
            }

            var response = await feedbackService.UpdateFeedbackAsync(feedbackUpdateDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Deletes a specific feedback entry.
        [Authorize]
        [HttpDelete("DeleteFeedback")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteFeedback([FromBody] FeedbackDeleteRequest feedbackDeleteDto)
        {
            var currentCustomerId = User.GetCustomerId();
            var isAdmin = User.IsAdmin();

            if (!isAdmin && currentCustomerId != feedbackDeleteDto.CustomerId)
            {
                return Forbid();
            }

            var response = await feedbackService.DeleteFeedbackAsync(feedbackDeleteDto, isAdmin);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
