using ECommerceApp.Commons;
using ECommerceApp.DTOs.RefundDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RefundsController(IRefundService refundService) : ControllerBase
    {
        // GET: api/Refunds/GetEligibleRefunds
        // Returns approved cancellations that have no associated refund entry.
        [HttpGet("GetEligibleRefunds")]
        public async Task<ActionResult<ApiResponse<PagedResult<PendingRefundResponse>>>> GetEligibleRefunds([FromQuery] PaginationRequest paginationRequest)
        {
            var response = await refundService.GetEligibleRefundsAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // POST: api/Refunds/ProcessRefund
        // Initiates a refund for approved cancellations without an existing refund record.
        [HttpPost("ProcessRefund/{cancellationId:int}")]
        public async Task<ActionResult<ApiResponse<RefundResponse>>> ProcessRefund(int cancellationId, [FromBody] RefundRequest refundRequest)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await refundService.ProcessRefundAsync(cancellationId, currentCustomerId.Value, refundRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // PUT: api/Refunds/UpdateRefundStatus
        // Manually reprocesses a refund (only applicable if the refund is pending or failed).
        [HttpPut("UpdateRefundStatus/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateRefundStatus(int id, [FromBody] RefundStatusUpdateRequest statusUpdate)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await refundService.UpdateRefundStatusAsync(id, currentCustomerId.Value, statusUpdate);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // GET: api/Refunds/GetRefundById/{id}
        // Retrieves a refund by its ID.
        [HttpGet("GetRefundById/{id}")]
        public async Task<ActionResult<ApiResponse<RefundResponse>>> GetRefundById(int id)
        {
            var response = await refundService.GetRefundByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // GET: api/Refunds/GetAllRefunds
        // Retrieves all refunds.
        [HttpGet("GetAllRefunds")]
        public async Task<ActionResult<ApiResponse<PagedResult<RefundResponse>>>> GetAllRefunds([FromQuery] PaginationRequest paginationRequest)
        {
            var response = await refundService.GetAllRefundsAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
