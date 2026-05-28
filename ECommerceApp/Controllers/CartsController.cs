using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ShoppingCartDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public CartsController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [Authorize]
        [HttpGet("GetCart/{customerId}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> GetCartByCustomerId(int customerId)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _shoppingCartService.GetCartByCustomerIdAsync(customerId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Adds an item to the customer's cart.
        [Authorize]
        [HttpPost("AddToCart/{customerId:int}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> AddToCart(int customerId, [FromBody] AddToCartRequest addToCartDTO)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _shoppingCartService.AddToCartAsync(customerId, addToCartDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates the quantity of an existing cart item.
        [Authorize]
        [HttpPut("UpdateCartItem/{customerId:int}/items/{cartItemId:int}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> UpdateCartItem(int customerId, int cartItemId, [FromBody] UpdateCartItemRequest updateCartItemDTO)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _shoppingCartService.UpdateCartItemAsync(customerId, cartItemId, updateCartItemDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Removes a specific item from the cart.
        [Authorize]
        [HttpDelete("RemoveCartItem/{customerId:int}/items/{cartItemId:int}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> RemoveCartItem(int customerId, int cartItemId)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _shoppingCartService.RemoveCartItemAsync(customerId, cartItemId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Clears all items from the customer's active cart.
        [Authorize]
        [HttpDelete("ClearCart/{customerId:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> ClearCart(int customerId)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _shoppingCartService.ClearCartAsync(customerId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
