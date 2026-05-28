using ECommerceApp.Commons;
using ECommerceApp.DTOs.FeedbackDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class FeedbackService(IUnitOfWork unitOfWork, ILogger<FeedbackService> logger) : IFeedbackService
    {
        public async Task<ApiResponse<FeedbackResponse>> SubmitFeedbackAsync(FeedbackCreateRequest feedbackCreateRequest)
        {
            try
            {
                var customer = await unitOfWork.CustomerRepository.GetActiveByIdAsync(feedbackCreateRequest.CustomerId, trackChanges: false);
                if (customer == null)
                {
                    return new ApiResponse<FeedbackResponse>(404, "Customer not found.");
                }

                var product = await unitOfWork.ProductRepository.GetByIdAsync(feedbackCreateRequest.ProductId, trackChanges: false);
                if (product == null)
                {
                    return new ApiResponse<FeedbackResponse>(404, "Product not found.");
                }

                bool hasPurchased = await unitOfWork.OrderRepository.HasCustomerPurchasedAndReceivedProductAsync(
                    feedbackCreateRequest.CustomerId, 
                    feedbackCreateRequest.ProductId);

                if (!hasPurchased)
                {
                    return new ApiResponse<FeedbackResponse>(400, "Customer must have purchased and received the product to submit feedback.");
                }

                bool feedbackExists = await unitOfWork.FeedbackRepository.ExistsByCustomerAndProductAsync(
                    feedbackCreateRequest.CustomerId, 
                    feedbackCreateRequest.ProductId);

                if (feedbackExists)
                {
                    return new ApiResponse<FeedbackResponse>(400, "Feedback for this product by this customer already exists.");
                }

                var feedback = new Feedback
                {
                    CustomerId = feedbackCreateRequest.CustomerId,
                    ProductId = feedbackCreateRequest.ProductId,
                    Rating = feedbackCreateRequest.Rating,
                    Comment = feedbackCreateRequest.Comment,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                unitOfWork.FeedbackRepository.Add(feedback);
                
                await unitOfWork.SaveChangesAsync();

                var feedbackResponse = new FeedbackResponse
                {
                    Id = feedback.Id,
                    CustomerId = customer.Id,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    CreatedAt = feedback.CreatedAt,
                    UpdatedAt = feedback.UpdatedAt
                };

                return new ApiResponse<FeedbackResponse>(200, feedbackResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in FeedbackService.");
                return new ApiResponse<FeedbackResponse>(500, $"An unexpected error occurred while submitting feedback: {ex.Message}");
            }
        }
        
        // Retrieves all feedback for a specific product along with the average rating.
        public async Task<ApiResponse<ProductFeedbackResponse>> GetFeedbackForProductAsync(int productId, PaginationRequest paginationRequest)
        {
            try
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(productId, trackChanges: false);

                if (product == null)
                {
                    return new ApiResponse<ProductFeedbackResponse>(404, "Product not found.");
                }

                var feedbacks = await unitOfWork.FeedbackRepository.GetByProductIdWithCustomerAsync(productId, trackChanges: false);

                double averageRating = 0;
                var customerFeedbacks = new List<CustomerFeedback>();

                if (feedbacks.Any())
                {
                    averageRating = feedbacks.Average(f => f.Rating);
                    customerFeedbacks = feedbacks.Select(f => new CustomerFeedback
                    {
                        Id = f.Id,
                        CustomerId = f.CustomerId,
                        CustomerName = $"{f.Customer.FirstName} {f.Customer.LastName}",
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedAt = f.CreatedAt,
                        UpdatedAt = f.UpdatedAt
                    }).ToList();
                }

                var productFeedbackResponse = new ProductFeedbackResponse
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    AverageRating = Math.Round(averageRating, 2),
                    Feedbacks = customerFeedbacks.ToPagedResult(paginationRequest)
                };

                return new ApiResponse<ProductFeedbackResponse>(200, productFeedbackResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in FeedbackService.");
                return new ApiResponse<ProductFeedbackResponse>(500, $"An unexpected error occurred while retrieving feedbacks: {ex.Message}");
            }
        }

        // Retrieves all feedback entries in the system.
        public async Task<ApiResponse<PagedResult<FeedbackResponse>>> GetAllFeedbackAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var feedbacks = await unitOfWork.FeedbackRepository.GetAllWithDetailsAsync(trackChanges: false);

                var feedbackResponseList = feedbacks.Select(f => new FeedbackResponse
                {
                    Id = f.Id,
                    CustomerId = f.CustomerId,
                    CustomerName = $"{f.Customer?.FirstName} {f.Customer?.LastName}",
                    ProductId = f.ProductId,
                    ProductName = f.Product?.Name,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList();

                return new ApiResponse<PagedResult<FeedbackResponse>>(200, feedbackResponseList.ToPagedResult(paginationRequest));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in FeedbackService.");
                return new ApiResponse<PagedResult<FeedbackResponse>>(500, $"An unexpected error occurred while retrieving all feedback: {ex.Message}");
            }
        }
        
        // Updates an existing feedback entry.
        public async Task<ApiResponse<FeedbackResponse>> UpdateFeedbackAsync(FeedbackUpdateRequest feedbackUpdateRequest)
        {
            try
            {
                var feedback = await unitOfWork.FeedbackRepository.GetByIdAndCustomerIdWithDetailsAsync(
                    feedbackUpdateRequest.FeedbackId,
                    feedbackUpdateRequest.CustomerId,
                    trackChanges: true);

                if (feedback == null)
                {
                    return new ApiResponse<FeedbackResponse>(404, "Feedback not found or you do not have permission to update it.");
                }

                feedback.Rating = feedbackUpdateRequest.Rating;
                feedback.Comment = feedbackUpdateRequest.Comment;
                feedback.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.SaveChangesAsync();

                var feedbackResponse = new FeedbackResponse
                {
                    Id = feedback.Id,
                    CustomerId = feedback.CustomerId,
                    CustomerName = $"{feedback.Customer?.FirstName} {feedback.Customer?.LastName}",
                    ProductId = feedback.ProductId,
                    ProductName = feedback.Product?.Name,
                    Rating = feedback.Rating,
                    Comment = feedback.Comment,
                    CreatedAt = feedback.CreatedAt,
                    UpdatedAt = feedback.UpdatedAt
                };

                return new ApiResponse<FeedbackResponse>(200, feedbackResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in FeedbackService.");
                return new ApiResponse<FeedbackResponse>(500, $"An unexpected error occurred while updating the feedback: {ex.Message}");
            }
        }

        // Deletes a feedback entry.
        public async Task<ApiResponse<ConfirmationResponse>> DeleteFeedbackAsync(FeedbackDeleteRequest feedbackDeleteRequest, bool isAdmin = false)
        {
            try
            {
                var feedback = await unitOfWork.FeedbackRepository.GetByIdAsync(feedbackDeleteRequest.FeedbackId, trackChanges: true);

                if (feedback == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Feedback not found.");
                }

                if (!isAdmin && feedback.CustomerId != feedbackDeleteRequest.CustomerId)
                {
                    return new ApiResponse<ConfirmationResponse>(401, "You are not authorized to delete this feedback.");
                }

                unitOfWork.FeedbackRepository.Remove(feedback);
                await unitOfWork.SaveChangesAsync();

                var confirmation = new ConfirmationResponse
                {
                    Message = $"Feedback with Id {feedbackDeleteRequest.FeedbackId} was deleted successfully."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in FeedbackService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while deleting the feedback: {ex.Message}");
            }
        }
    }
}
