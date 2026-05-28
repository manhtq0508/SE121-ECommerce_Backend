using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.CancellationDTOs
{
    public class CancellationRequest
    {
        [Required(ErrorMessage = "Cancellation reason is required.")]
        [StringLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters.")]
        public string Reason { get; set; }
    }
}
