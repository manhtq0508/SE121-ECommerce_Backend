using ECommerceApp.Commons;
using ECommerceApp.DTOs.FeedbackDTOs;

namespace ECommerceApp.Services.Interfaces;

public interface IFeedbackService
{
    Task<ApiResponse<FeedbackResponse>> SubmitFeedbackAsync(FeedbackCreateRequest feedbackCreateRequest);
    Task<ApiResponse<ProductFeedbackResponse>> GetFeedbackForProductAsync(int productId, PaginationRequest paginationRequest);
    Task<ApiResponse<PagedResult<FeedbackResponse>>> GetAllFeedbackAsync(PaginationRequest paginationRequest);
    Task<ApiResponse<FeedbackResponse>> UpdateFeedbackAsync(FeedbackUpdateRequest feedbackUpdateRequest);
    Task<ApiResponse<ConfirmationResponse>> DeleteFeedbackAsync(FeedbackDeleteRequest feedbackDeleteRequest, bool isAdmin = false);
}
