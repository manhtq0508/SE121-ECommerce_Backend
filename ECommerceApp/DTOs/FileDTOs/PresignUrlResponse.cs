namespace ECommerceApp.DTOs.FileDTOs
{
    public class PresignUrlResponse
    {
        public string UploadUrl { get; set; } = null!;
        public string FileKey { get; set; } = null!;
        public string ContentType { get; set; } = null!;
    }
}
