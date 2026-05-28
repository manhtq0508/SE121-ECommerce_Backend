using System.Security.Claims;
using ECommerceApp.Commons;
using ECommerceApp.DTOs.FileDTOs;
using ECommerceApp.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace ECommerceApp.Services.Implements
{
    public class FileService(IConfiguration configuration, ILogger<FileService> logger) : IFileService
    {
        private const int SignedUrlExpiresInSeconds = 900;
        private static readonly TimeSpan SignedUrlDuration = TimeSpan.FromSeconds(SignedUrlExpiresInSeconds);

        public ApiResponse<PresignUrlResponse> GetUploadUrl(string? fileName, string? contentType, ClaimsPrincipal user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType))
                {
                    return new ApiResponse<PresignUrlResponse>(StatusCodes.Status400BadRequest, "Invalid upload url request.");
                }

                var normalizedContentType = contentType.Trim();
                if (!IsImageContentType(normalizedContentType))
                {
                    return new ApiResponse<PresignUrlResponse>(StatusCodes.Status400BadRequest, "Only image uploads are supported.");
                }

                var objectName = CreateUploadObjectName(fileName, user);
                if (string.IsNullOrWhiteSpace(objectName))
                {
                    return new ApiResponse<PresignUrlResponse>(StatusCodes.Status401Unauthorized, "Invalid user context.");
                }

                return new ApiResponse<PresignUrlResponse>(
                    StatusCodes.Status200OK,
                    CreateUploadUrl(objectName, normalizedContentType));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while creating a signed upload URL.");
                return new ApiResponse<PresignUrlResponse>(StatusCodes.Status500InternalServerError, $"An unexpected error occurred while creating the upload url, Error: {ex.Message}");
            }
        }

        public ApiResponse<PresignReadUrlResponse> GetImageUrl(string? fileKey, ClaimsPrincipal user)
        {
            try
            {
                var normalizedFileKey = NormalizeFileKey(fileKey);

                if (string.IsNullOrWhiteSpace(normalizedFileKey))
                {
                    return new ApiResponse<PresignReadUrlResponse>(StatusCodes.Status400BadRequest, "File key is required.");
                }

                if (!CanReadFile(normalizedFileKey, user))
                {
                    return new ApiResponse<PresignReadUrlResponse>(StatusCodes.Status403Forbidden, "You do not have permission to read this file.");
                }

                return new ApiResponse<PresignReadUrlResponse>(
                    StatusCodes.Status200OK,
                    CreateReadUrl(normalizedFileKey));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while creating a signed image URL.");
                return new ApiResponse<PresignReadUrlResponse>(StatusCodes.Status500InternalServerError, $"An unexpected error occurred while creating the image url, Error: {ex.Message}");
            }
        }

        private PresignUrlResponse CreateUploadUrl(string objectName, string contentType)
        {
            var requestTemplate = CreateRequestTemplate(objectName, HttpMethod.Put)
                .WithContentHeaders(new Dictionary<string, IEnumerable<string>>
                {
                    { "Content-Type", new[] { contentType } }
                });

            return new PresignUrlResponse
            {
                FileKey = objectName,
                UploadUrl = Sign(requestTemplate),
                ContentType = contentType
            };
        }

        private PresignReadUrlResponse CreateReadUrl(string objectName)
        {
            var requestTemplate = CreateRequestTemplate(objectName, HttpMethod.Get);

            return new PresignReadUrlResponse
            {
                FileKey = objectName,
                ReadUrl = Sign(requestTemplate),
                ExpiresInSeconds = SignedUrlExpiresInSeconds
            };
        }

        private UrlSigner.RequestTemplate CreateRequestTemplate(string objectName, HttpMethod httpMethod)
        {
            var bucketName = configuration["BUCKET_NAME"] ?? configuration["GoogleCloud:BucketName"];

            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new InvalidOperationException("BUCKET_NAME configuration is missing.");
            }

            return UrlSigner.RequestTemplate
                .FromBucket(bucketName)
                .WithObjectName(objectName)
                .WithHttpMethod(httpMethod);
        }

        private static string Sign(UrlSigner.RequestTemplate requestTemplate)
        {
            var credential = GoogleCredential.GetApplicationDefault();
            var urlSigner = UrlSigner.FromCredential(credential);
            var options = UrlSigner.Options.FromDuration(SignedUrlDuration);

            return urlSigner.Sign(requestTemplate, options);
        }

        private static string? CreateUploadObjectName(string fileName, ClaimsPrincipal user)
        {
            var safeFileName = CreateSafeFileName(fileName);

            if (IsCurrentUserAdmin(user))
            {
                return $"products/{safeFileName}";
            }

            var customerId = GetCurrentCustomerId(user);
            return customerId.HasValue ? $"users/{customerId}/{safeFileName}" : null;
        }

        private static string CreateSafeFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return $"{Guid.NewGuid()}{extension}";
        }

        private static bool CanReadFile(string fileKey, ClaimsPrincipal user)
        {
            if (fileKey.StartsWith("products/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (IsCurrentUserAdmin(user))
            {
                return true;
            }

            var customerId = GetCurrentCustomerId(user);
            return customerId.HasValue && fileKey.StartsWith($"users/{customerId}/", StringComparison.OrdinalIgnoreCase);
        }

        private static int? GetCurrentCustomerId(ClaimsPrincipal user)
        {
            var customerIdClaim = user.FindFirstValue("customer_id")
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub")
                ?? user.FindFirstValue("userId");

            return int.TryParse(customerIdClaim, out var customerId) ? customerId : null;
        }

        private static bool IsCurrentUserAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        private static bool IsImageContentType(string contentType)
        {
            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeFileKey(string? fileKey)
        {
            return string.IsNullOrWhiteSpace(fileKey)
                ? string.Empty
                : fileKey.Trim().TrimStart('/').Replace('\\', '/');
        }
    }
}
