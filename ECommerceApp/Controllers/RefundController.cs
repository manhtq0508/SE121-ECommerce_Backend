using ECommerceApp.Commons;
using ECommerceApp.DTOs.RefundDTOs;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RefundsController(IRefundService refundService) : ControllerBase
    {
        // GET: api/Refunds/GetEligibleRefunds
        // Returns approved cancellations that have no associated refund entry.
        [HttpGet("GetEligibleRefunds")]
        public async Task<ActionResult<ApiResponse<List<PendingRefundResponse>>>> GetEligibleRefunds()
        {
            var response = await refundService.GetEligibleRefundsAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // POST: api/Refunds/ProcessRefund
        // Initiates a refund for approved cancellations without an existing refund record.
        [HttpPost("ProcessRefund")]
        public async Task<ActionResult<ApiResponse<RefundResponse>>> ProcessRefund([FromBody] RefundRequest refundRequest)
        {
            var response = await refundService.ProcessRefundAsync(refundRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // PUT: api/Refunds/UpdateRefundStatus
        // Manually reprocesses a refund (only applicable if the refund is pending or failed).
        [HttpPut("UpdateRefundStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateRefundStatus([FromBody] RefundStatusUpdateRequest statusUpdate)
        {
            var response = await refundService.UpdateRefundStatusAsync(statusUpdate);
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
        public async Task<ActionResult<ApiResponse<List<RefundResponse>>>> GetAllRefunds()
        {
            var response = await refundService.GetAllRefundsAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
