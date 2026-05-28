using ECommerceApp.Commons;
using ECommerceApp.DTOs.OrderDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateOrderAsync(int customerId, OrderCreateRequest orderDto);
    Task<ApiResponse<OrderResponse>> GetOrderByIdAsync(int orderId);
    Task<ApiResponse<ConfirmationResponse>> UpdateOrderStatusAsync(int orderId, OrderStatusUpdateRequest statusDto);
    Task<ApiResponse<PagedResult<OrderResponse>>> GetAllOrdersAsync(PaginationRequest paginationRequest);
    Task<ApiResponse<PagedResult<OrderResponse>>> GetOrdersByCustomerAsync(int customerId, PaginationRequest paginationRequest);
}
