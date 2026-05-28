using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.PaymentDTOs
{
    public class PaymentRequest
    {
        [Required(ErrorMessage = "Payment Method is required.")]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
