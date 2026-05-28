using ECommerceApp.Commons;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.DTOs.CancellationDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CancellationsController : ControllerBase
    {
        private readonly ICancellationService _cancellationService;

        // Inject the CancellationService via constructor
        public CancellationsController(ICancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }

        // Endpoint for customers to request an order cancellation.
        [Authorize]
        [HttpPost("RequestCancellation/{orderId:int}")]
        public async Task<ActionResult<ApiResponse<CancellationResponse>>> RequestCancellation(int orderId, [FromBody] CancellationRequest cancellationRequest)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await _cancellationService.RequestCancellationAsync(orderId, currentCustomerId.Value, User.IsAdmin(), cancellationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Endpoint to retrieve all cancellation requests.
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllCancellations")]
        public async Task<ActionResult<ApiResponse<PagedResult<CancellationResponse>>>> GetAllCancellations([FromQuery] PaginationRequest paginationRequest)
        {
            var response = await _cancellationService.GetAllCancellationsAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Endpoint to retrieve cancellation details by cancellation ID.
        [Authorize(Roles = "Admin")]
        [HttpGet("GetCancellationById/{id}")]
        public async Task<ActionResult<ApiResponse<CancellationResponse>>> GetCancellationById(int id)
        {
            var response = await _cancellationService.GetCancellationByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Endpoint for administrators to update the status of a cancellation request.
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateCancellationStatus/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateCancellationStatus(int id, [FromBody] CancellationStatusUpdateRequest statusUpdate)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await _cancellationService.UpdateCancellationStatusAsync(id, currentCustomerId.Value, statusUpdate);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
