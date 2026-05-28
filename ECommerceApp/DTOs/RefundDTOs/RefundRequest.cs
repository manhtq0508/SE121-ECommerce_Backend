using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.RefundDTOs
{
    public class RefundRequest
    {
        [Required(ErrorMessage = "Refund Method is required.")]
        public RefundMethod RefundMethod { get; set; }

        [StringLength(500, ErrorMessage = "Refund Reason cannot exceed 500 characters.")]
        public string? RefundReason { get; set; }

    }
}
