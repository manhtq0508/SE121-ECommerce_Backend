using ECommerceApp.Commons;
using ECommerceApp.DTOs.PaymentDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IPaymentService
{
    Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(int orderId, int currentCustomerId, bool isAdmin, PaymentRequest paymentRequest);
    Task<ApiResponse<PaymentResponse>> GetPaymentByIdAsync(int paymentId);
    Task<ApiResponse<PaymentResponse>> GetPaymentByOrderIdAsync(int orderId);
    Task<ApiResponse<ConfirmationResponse>> UpdatePaymentStatusAsync(int paymentId, PaymentStatusUpdateRequest statusUpdate);
    Task<ApiResponse<ConfirmationResponse>> CompleteCodPaymentAsync(int paymentId);
    Task SendOrderConfirmationEmailAsync(int orderId);
}
