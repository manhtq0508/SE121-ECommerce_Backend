using ECommerceApp.Commons;
using ECommerceApp.DTOs.CancellationDTOs;
using ECommerceApp.Entities;

namespace ECommerceApp.Services.Interfaces;

public interface ICancellationService
{
    Task<ApiResponse<CancellationResponse>> RequestCancellationAsync(int orderId, int currentCustomerId, bool isAdmin, CancellationRequest cancellationRequest);
    Task<ApiResponse<CancellationResponse>> GetCancellationByIdAsync(int id);
    Task<ApiResponse<ConfirmationResponse>> UpdateCancellationStatusAsync(int cancellationId, int processedBy, CancellationStatusUpdateRequest statusUpdate);
    Task<ApiResponse<PagedResult<CancellationResponse>>> GetAllCancellationsAsync(PaginationRequest paginationRequest);
}
