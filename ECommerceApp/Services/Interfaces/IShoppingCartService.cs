using ECommerceApp.Commons;
using ECommerceApp.DTOs.ShoppingCartDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ApiResponse<CartResponse>> GetCartByCustomerIdAsync(int customerId);
        Task<ApiResponse<CartResponse>> AddToCartAsync(int customerId, AddToCartRequest addToCartDto);
        Task<ApiResponse<CartResponse>> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartItemRequest updateCartItemDto);
        Task<ApiResponse<CartResponse>> RemoveCartItemAsync(int customerId, int cartItemId);
        Task<ApiResponse<ConfirmationResponse>> ClearCartAsync(int customerId);
    }
}
