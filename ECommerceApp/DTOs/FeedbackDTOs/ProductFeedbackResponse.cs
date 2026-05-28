using ECommerceApp.Commons;

namespace ECommerceApp.DTOs.FeedbackDTOs
{
    public class ProductFeedbackResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double AverageRating { get; set; }
        public PagedResult<CustomerFeedback> Feedbacks { get; set; } = new();
    }
}
