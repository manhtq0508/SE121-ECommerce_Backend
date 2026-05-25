using ECommerceApp.Commons;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.OrderDTOs;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {

        // Creates a new order.
        // POST: api/Orders/CreateOrder
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateOrder([FromBody] OrderCreateRequest orderDto)
        {
            var response = await orderService.CreateOrderAsync(orderDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves an order by its ID.
        // GET: api/Orders/GetOrderById/{id}
        [HttpGet("GetOrderById/{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrderById(int id)
        {
            var response = await orderService.GetOrderByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates the status of an existing order.
        // PUT: api/Orders/UpdateOrderStatus
        [HttpPut("UpdateOrderStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateOrderStatus([FromBody] OrderStatusUpdateRequest statusDto)
        {
            var response = await orderService.UpdateOrderStatusAsync(statusDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all orders.
        // GET: api/Orders/GetAllOrders
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<ApiResponse<List<OrderResponse>>>> GetAllOrders()
        {
            var response = await orderService.GetAllOrdersAsync();
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all orders for a specific customer.
        // GET: api/Orders/GetOrdersByCustomer/{customerId}
        [HttpGet("GetOrdersByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<List<OrderResponse>>>> GetOrdersByCustomer(int customerId)
        {
            var response = await orderService.GetOrdersByCustomerAsync(customerId);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
