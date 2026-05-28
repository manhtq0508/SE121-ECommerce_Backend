using System.ComponentModel.DataAnnotations;
namespace ECommerceApp.DTOs.ProductDTOs
{
    public class ProductStatusUpdateRequest
    {
        [Required(ErrorMessage = "IsAvailable is required.")]
        public bool IsAvailable { get; set; }
    }
}
