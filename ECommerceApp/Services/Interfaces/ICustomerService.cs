using ECommerceApp.Commons;
using ECommerceApp.DTOs.CustomerDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiResponse<CustomerResponse>> RegisterCustomerAsync(CustomerRegistrationRequest customerDto);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int id);
        Task<ApiResponse<ConfirmationResponse>> UpdateCustomerAsync(int customerId, CustomerUpdateRequest customerDto);
        Task<ApiResponse<ConfirmationResponse>> DeleteCustomerAsync(int id);
        Task<ApiResponse<ConfirmationResponse>> ChangePasswordAsync(int customerId, ChangePasswordRequest request);
    }
}
