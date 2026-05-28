using ECommerceApp.Commons;
using ECommerceApp.DTOs.RefundDTOs;
using ECommerceApp.Entities;

namespace ECommerceApp.Services.Interfaces;

public interface IRefundService
{
    Task<ApiResponse<PagedResult<PendingRefundResponse>>> GetEligibleRefundsAsync(PaginationRequest paginationRequest);
    Task<ApiResponse<RefundResponse>> ProcessRefundAsync(int cancellationId, int processedBy, RefundRequest refundRequest);
    Task<ApiResponse<ConfirmationResponse>> UpdateRefundStatusAsync(int refundId, int processedBy, RefundStatusUpdateRequest statusUpdate);
    Task<ApiResponse<RefundResponse>> GetRefundByIdAsync(int id);
    Task<ApiResponse<PagedResult<RefundResponse>>> GetAllRefundsAsync(PaginationRequest paginationRequest);
    Task<PaymentGatewayRefundResponse> ProcessRefundPaymentAsync(Refund refund);
    string GenerateRefundSuccessEmailBody(Refund refund, string orderNumber, Cancellation cancellation);
}
