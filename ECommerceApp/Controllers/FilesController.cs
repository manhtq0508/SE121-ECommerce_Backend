using ECommerceApp.Commons;
using ECommerceApp.DTOs.FileDTOs;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFileService fileService) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-upload-url")]
        public ActionResult<ApiResponse<PresignUrlResponse>> GetUploadUrl([FromQuery] string fileName, [FromQuery] string contentType)
        {
            var response = fileService.GetUploadUrl(fileName, contentType, User);
            return ToActionResult(response);
        }

        [AllowAnonymous]
        [HttpGet("get-image-url")]
        public ActionResult<ApiResponse<PresignReadUrlResponse>> GetImageUrl([FromQuery] string? fileKey)
        {
            var response = fileService.GetImageUrl(fileKey, User);
            return ToActionResult(response);
        }

        private ActionResult<ApiResponse<T>> ToActionResult<T>(ApiResponse<T> response)
        {
            if (response.StatusCode != StatusCodes.Status200OK)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }
    }
}
