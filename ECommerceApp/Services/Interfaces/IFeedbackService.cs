using ECommerceApp.Commons;
using ECommerceApp.DTOs.FeedbackDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IFeedbackService
{
    Task<ApiResponse<FeedbackResponse>> SubmitFeedbackAsync(int customerId, int productId, FeedbackCreateRequest feedbackCreateRequest);
    Task<ApiResponse<ProductFeedbackResponse>> GetFeedbackForProductAsync(int productId, PaginationRequest paginationRequest);
    Task<ApiResponse<PagedResult<FeedbackResponse>>> GetAllFeedbackAsync(PaginationRequest paginationRequest);
    Task<ApiResponse<FeedbackResponse>> UpdateFeedbackAsync(int feedbackId, int currentCustomerId, bool isAdmin, FeedbackUpdateRequest feedbackUpdateRequest);
    Task<ApiResponse<ConfirmationResponse>> DeleteFeedbackAsync(int feedbackId, int currentCustomerId, bool isAdmin = false);
}
