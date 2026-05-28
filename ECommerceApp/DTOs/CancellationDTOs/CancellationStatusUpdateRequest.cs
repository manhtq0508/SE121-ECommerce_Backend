using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;
namespace ECommerceApp.DTOs.CancellationDTOs
{
    public class CancellationStatusUpdateRequest
    {
        [Required]
        public CancellationStatus Status { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cancellation charges must be non-negative.")]
        public decimal? CancellationCharges { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string Remarks { get; set; }
    }
}
