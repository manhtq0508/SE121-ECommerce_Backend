using ECommerceApp.Commons;
using ECommerceApp.DTOs.CustomerDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [AllowAnonymous]
        [HttpPost("RegisterCustomer")]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> RegisterCustomer([FromBody] CustomerRegistrationRequest customerDto)
        {
            var response = await _customerService.RegisterCustomerAsync(customerDto);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            var response = await _customerService.LoginAsync(request);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetCustomerById/{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> GetCustomerById(int id)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != id)
            {
                return Forbid();
            }

            var response = await _customerService.GetCustomerByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPut("UpdateCustomer/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateCustomer(int id, [FromBody] CustomerUpdateRequest request)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != id)
            {
                return Forbid();
            }

            var response = await _customerService.UpdateCustomerAsync(id, request);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteCustomer(int id)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != id)
            {
                return Forbid();
            }

            var response = await _customerService.DeleteCustomerAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await _customerService.ChangePasswordAsync(currentCustomerId.Value, request);
            if (response.StatusCode != 200)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
