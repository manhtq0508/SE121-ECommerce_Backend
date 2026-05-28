using ECommerceApp.Commons;
using ECommerceApp.DTOs.OrderDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<OrderResponse>> CreateOrderAsync(OrderCreateRequest orderDto);
    Task<ApiResponse<OrderResponse>> GetOrderByIdAsync(int orderId);
    Task<ApiResponse<ConfirmationResponse>> UpdateOrderStatusAsync(OrderStatusUpdateRequest statusDto);
    Task<ApiResponse<PagedResult<OrderResponse>>> GetAllOrdersAsync(PaginationRequest paginationRequest);
    Task<ApiResponse<PagedResult<OrderResponse>>> GetOrdersByCustomerAsync(int customerId, PaginationRequest paginationRequest);
}
