using ECommerceApp.Commons;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.DTOs.FeedbackDTOs;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController(IFeedbackService feedbackService) : ControllerBase
    {
        // Submits feedback for a product.
        [HttpPost("SubmitFeedback")]
        public async Task<ActionResult<ApiResponse<FeedbackResponse>>> SubmitFeedback([FromBody] FeedbackCreateRequest feedbackCreateDto)
        {
            var response = await feedbackService.SubmitFeedbackAsync(feedbackCreateDto);

            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all feedback for a specific product.
        [HttpGet("GetFeedbackForProduct/{productId}")]
        public async Task<ActionResult<ApiResponse<ProductFeedbackResponse>>> GetFeedbackForProduct(int productId)
        {
            var response = await feedbackService.GetFeedbackForProductAsync(productId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all feedback (Admin use).
        [HttpGet("GetAllFeedback")]
        public async Task<ActionResult<ApiResponse<List<FeedbackResponse>>>> GetAllFeedback()
        {
            var response = await feedbackService.GetAllFeedbackAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates a specific feedback entry.
        [HttpPut("UpdateFeedback")]
        public async Task<ActionResult<ApiResponse<FeedbackResponse>>> UpdateFeedback([FromBody] FeedbackUpdateRequest feedbackUpdateDto)
        {
            var response = await feedbackService.UpdateFeedbackAsync(feedbackUpdateDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Deletes a specific feedback entry.
        [HttpDelete("DeleteFeedback")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteFeedback([FromBody] FeedbackDeleteRequest feedbackDeleteDto)
        {
            var response = await feedbackService.DeleteFeedbackAsync(feedbackDeleteDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
