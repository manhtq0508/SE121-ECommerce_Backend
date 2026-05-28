using System.Security.Claims;
using ECommerceApp.Commons;
using ECommerceApp.DTOs.FileDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface IFileService
    {
        ApiResponse<PresignUrlResponse> GetUploadUrl(string? fileName, string? contentType, ClaimsPrincipal user);
        ApiResponse<PresignReadUrlResponse> GetImageUrl(string? fileKey, ClaimsPrincipal user);
    }
}
