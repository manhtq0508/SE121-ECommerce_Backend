using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // Creates a new address for a customer.
        [Authorize]
        [HttpPost("CreateAddress/{customerId:int}")]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> CreateAddress(int customerId, [FromBody] AddressCreateRequest addressDto)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _addressService.CreateAddressAsync(customerId, addressDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves an address by ID.
        [Authorize]
        [HttpGet("GetAddressById/{id}")]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> GetAddressById(int id)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await _addressService.GetAddressByIdAsync(id, currentCustomerId.Value, User.IsAdmin());
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates an existing address.
        [Authorize]
        [HttpPut("UpdateAddress/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateAddress(int id, [FromBody] AddressUpdateRequest addressDto)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await _addressService.UpdateAddressAsync(id, currentCustomerId.Value, User.IsAdmin(), addressDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Deletes an address by ID.
        [Authorize]
        [HttpDelete("DeleteAddress/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteAddress(int id)
        {
            var currentCustomerId = User.GetCustomerId();
            if (currentCustomerId == null)
            {
                return Forbid();
            }

            var response = await _addressService.DeleteAddressAsync(id, currentCustomerId.Value, User.IsAdmin());
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all addresses for a specific customer.
        [Authorize]
        [HttpGet("GetAddressesByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<PagedResult<AddressResponse>>>> GetAddressesByCustomer(int customerId, [FromQuery] PaginationRequest paginationRequest)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _addressService.GetAddressesByCustomerAsync(customerId, paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
