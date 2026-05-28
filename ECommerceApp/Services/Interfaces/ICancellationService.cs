using ECommerceApp.Commons;
using ECommerceApp.DTOs.CancellationDTOs;
using ECommerceApp.Entities;

namespace ECommerceApp.Services.Interfaces;

public interface ICancellationService
{
    Task<ApiResponse<CancellationResponse>> RequestCancellationAsync(CancellationRequest cancellationRequest);
    Task<ApiResponse<CancellationResponse>> GetCancellationByIdAsync(int id);
    Task<ApiResponse<ConfirmationResponse>> UpdateCancellationStatusAsync(CancellationStatusUpdateRequest statusUpdate);
    Task<ApiResponse<PagedResult<CancellationResponse>>> GetAllCancellationsAsync(PaginationRequest paginationRequest);
}
