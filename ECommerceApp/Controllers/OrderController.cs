using ECommerceApp.Commons;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.OrderDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {

        // Creates a new order.
        // POST: api/Orders/CreateOrder
        [Authorize]
        [HttpPost("CreateOrder/{customerId:int}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateOrder(int customerId, [FromBody] OrderCreateRequest orderDto)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await orderService.CreateOrderAsync(customerId, orderDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves an order by its ID.
        // GET: api/Orders/GetOrderById/{id}
        [Authorize]
        [HttpGet("GetOrderById/{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrderById(int id)
        {
            var response = await orderService.GetOrderByIdAsync(id);
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

        // Updates the status of an existing order.
        // PUT: api/Orders/UpdateOrderStatus
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateOrderStatus/{id:int}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdateRequest statusDto)
        {
            var response = await orderService.UpdateOrderStatusAsync(id, statusDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all orders.
        // GET: api/Orders/GetAllOrders
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderResponse>>>> GetAllOrders([FromQuery] PaginationRequest paginationRequest)
        {
            var response = await orderService.GetAllOrdersAsync(paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all orders for a specific customer.
        // GET: api/Orders/GetOrdersByCustomer/{customerId}
        [Authorize]
        [HttpGet("GetOrdersByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderResponse>>>> GetOrdersByCustomer(int customerId, [FromQuery] PaginationRequest paginationRequest)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await orderService.GetOrdersByCustomerAsync(customerId, paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
