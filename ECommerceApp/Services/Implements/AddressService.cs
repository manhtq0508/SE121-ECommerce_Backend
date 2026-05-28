using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Addresses;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.Implements;

public class AddressService(
    IUnitOfWork unitOfWork,
    IAddressMapper mapper,
    ILogger<AddressService> logger) : IAddressService
{
    public async Task<ApiResponse<AddressResponse>> CreateAddressAsync(int customerId, AddressCreateRequest addressDto)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetActiveByIdAsync(customerId);
            if (customer == null)
            {
                return new ApiResponse<AddressResponse>(404, "Customer not found.");
            }

            var address = mapper.Map(addressDto);
            address.CustomerId = customerId;

            unitOfWork.AddressRepository.Add(address);
            await unitOfWork.SaveChangesAsync();

            var addressResponse = mapper.Map(address);

            return new ApiResponse<AddressResponse>(200, addressResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in AddressService.");
            return new ApiResponse<AddressResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int id, int currentCustomerId, bool isAdmin)
    {
        try
        {
            var address = await unitOfWork.AddressRepository.GetByIdAsync(id, trackChanges: false);

            if (address == null)
            {
                return new ApiResponse<AddressResponse>(404, "Address not found.");
            }

            if (!isAdmin && address.CustomerId != currentCustomerId)
            {
                return new ApiResponse<AddressResponse>(403, "You do not have permission to access this address.");
            }

            var addressResponse = mapper.Map(address);
            return new ApiResponse<AddressResponse>(200, addressResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in AddressService.");
            return new ApiResponse<AddressResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> UpdateAddressAsync(int addressId, int currentCustomerId, bool isAdmin, AddressUpdateRequest addressDto)
    {
        try
        {
            var address = await unitOfWork.AddressRepository.GetByIdAsync(addressId, trackChanges: true);

            if (address == null)
            {
                return new ApiResponse<ConfirmationResponse>(404, "Address not found.");
            }

            if (!isAdmin && address.CustomerId != currentCustomerId)
            {
                return new ApiResponse<ConfirmationResponse>(403, "You do not have permission to update this address.");
            }

            address.AddressLine1 = addressDto.AddressLine1;
            address.AddressLine2 = addressDto.AddressLine2;
            address.City = addressDto.City;
            address.State = addressDto.State;
            address.PostalCode = addressDto.PostalCode;
            address.Country = addressDto.Country;

            await unitOfWork.SaveChangesAsync(); 

            var confirmationMessage = new ConfirmationResponse
            {
                Message = $"Address with Id {addressId} updated successfully."
            };

            return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in AddressService.");
            return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> DeleteAddressAsync(int addressId, int currentCustomerId, bool isAdmin)
    {
        try
        {
            var address = await unitOfWork.AddressRepository.GetByIdAsync(addressId, trackChanges: true);

            if (address == null)
            {
                return new ApiResponse<ConfirmationResponse>(404, "Address not found.");
            }

            if (!isAdmin && address.CustomerId != currentCustomerId)
            {
                return new ApiResponse<ConfirmationResponse>(403, "You do not have permission to delete this address.");
            }

            unitOfWork.AddressRepository.Remove(address);
            await unitOfWork.SaveChangesAsync();
            
            var confirmationMessage = new ConfirmationResponse
            {
                Message = $"Address with Id {addressId} deleted successfully."
            };

            return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in AddressService.");
            return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<AddressResponse>>> GetAddressesByCustomerAsync(int customerId, PaginationRequest paginationRequest)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return new ApiResponse<PagedResult<AddressResponse>>(404, "Customer not found.");
            }

            var safeRequest = new PaginationRequest
            {
                PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
            };

            var addressQuery = unitOfWork.AddressRepository.QueryByCustomerId(customerId);
            var totalCount = await addressQuery.CountAsync();
            var addresses = await addressQuery
                .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                .Take(safeRequest.PageSize)
                .ToListAsync();

            var addressList = new PagedResult<AddressResponse>(addresses.Select(mapper.Map), safeRequest, totalCount);

            return new ApiResponse<PagedResult<AddressResponse>>(200, addressList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in AddressService.");
            return new ApiResponse<PagedResult<AddressResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }
}
