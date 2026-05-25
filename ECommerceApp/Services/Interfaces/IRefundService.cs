using ECommerceApp.Commons;
using ECommerceApp.DTOs.RefundDTOs;
using ECommerceApp.Entities;

namespace ECommerceApp.Services.Interfaces;

public interface IRefundService
{
    Task<ApiResponse<List<PendingRefundResponse>>> GetEligibleRefundsAsync();
    Task<ApiResponse<RefundResponse>> ProcessRefundAsync(RefundRequest refundRequest);
    Task<ApiResponse<ConfirmationResponse>> UpdateRefundStatusAsync(RefundStatusUpdateRequest statusUpdate);
    Task<ApiResponse<RefundResponse>> GetRefundByIdAsync(int id);
    Task<ApiResponse<List<RefundResponse>>> GetAllRefundsAsync();
    Task<PaymentGatewayRefundResponse> ProcessRefundPaymentAsync(Refund refund);
    string GenerateRefundSuccessEmailBody(Refund refund, string orderNumber, Cancellation cancellation);
}
