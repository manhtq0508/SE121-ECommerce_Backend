using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.RefundDTOs
{
    public class RefundStatusUpdateRequest
    {
        [StringLength(100, ErrorMessage = "Transaction ID cannot exceed 100 characters.")]
        [Required(ErrorMessage = "TransactionId is required.")]
        public string TransactionId { get; set; }

        [Required(ErrorMessage = "Refund Method is required.")]
        public RefundMethod RefundMethod { get; set; }

        [StringLength(500, ErrorMessage = "Refund Reason cannot exceed 500 characters.")]
        public string? RefundReason { get; set; }
    }
}
