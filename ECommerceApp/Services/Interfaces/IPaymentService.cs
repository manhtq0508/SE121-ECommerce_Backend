using ECommerceApp.Commons;
using ECommerceApp.DTOs.PaymentDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IPaymentService
{
    Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(PaymentRequest paymentRequest);
    Task<ApiResponse<PaymentResponse>> GetPaymentByIdAsync(int paymentId);
    Task<ApiResponse<PaymentResponse>> GetPaymentByOrderIdAsync(int orderId);
    Task<ApiResponse<ConfirmationResponse>> UpdatePaymentStatusAsync(PaymentStatusUpdateRequest statusUpdate);
    Task<ApiResponse<ConfirmationResponse>> CompleteCodPaymentAsync(CODPaymentUpdateRequest codPaymentUpdateDto);
    Task SendOrderConfirmationEmailAsync(int orderId);
}
