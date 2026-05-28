using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.PaymentDTOs
{
    public class PaymentStatusUpdateRequest
    {
        public string? TransactionId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public PaymentStatus Status { get; set; } 
    }
}
