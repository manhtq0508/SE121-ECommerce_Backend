using ECommerceApp.Commons;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.DTOs.PaymentDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Processes a payment for an order.
        [Authorize]
        [HttpPost("ProcessPayment/{orderId:int}")]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> ProcessPayment(int orderId, [FromBody] PaymentRequest paymentRequest)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await _paymentService.ProcessPaymentAsync(orderId, currentCustomerId.Value, User.IsAdmin(), paymentRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves payment details by Payment ID.
        [Authorize]
        [HttpGet("GetPaymentById/{paymentId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPaymentById(int paymentId)
        {
            var response = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            if (!User.IsAdmin() && User.GetCustomerId() != response.Data.CustomerId)
            {
                return Forbid();
            }

            return Ok(response);
        }

        // Retrieves payment details associated with a specific order.
        [Authorize]
        [HttpGet("GetPaymentByOrderId/{orderId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPaymentByOrderId(int orderId)
        {
            var response = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            if (!User.IsAdmin() && User.GetCustomerId() != response.Data.CustomerId)
            {
                return Forbid();
            }

            return Ok(response);
        }

        // Updates the status of an existing payment.
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdatePaymentStatus/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdatePaymentStatus(int id, [FromBody] PaymentStatusUpdateRequest statusUpdate)
        {
            var response = await _paymentService.UpdatePaymentStatusAsync(id, statusUpdate);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Completes a Cash on Delivery (COD) payment.
        [Authorize(Roles = "Admin")]
        [HttpPost("CompleteCODPayment/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> CompleteCODPayment(int id)
        {
            var response = await _paymentService.CompleteCodPaymentAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
