using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface IAddressService
    {
        Task<ApiResponse<AddressResponse>> CreateAddressAsync(int customerId, AddressCreateRequest addressDto);
        Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int id, int currentCustomerId, bool isAdmin);
        Task<ApiResponse<ConfirmationResponse>> UpdateAddressAsync(int addressId, int currentCustomerId, bool isAdmin, AddressUpdateRequest addressDto);
        Task<ApiResponse<ConfirmationResponse>> DeleteAddressAsync(int addressId, int currentCustomerId, bool isAdmin);
        Task<ApiResponse<PagedResult<AddressResponse>>> GetAddressesByCustomerAsync(int customerId, PaginationRequest paginationRequest);
    }
}
