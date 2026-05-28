using ECommerceApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.DTOs.OrderDTOs
{
    public class OrderStatusUpdateRequest
    {
        [Required]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid Order Status.")]
        public OrderStatus OrderStatus { get; set; }
    }
}
