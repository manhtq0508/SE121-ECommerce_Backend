namespace ECommerceApp.DTOs.FileDTOs
{
    public class PresignReadUrlResponse
    {
        public string ReadUrl { get; set; } = null!;
        public string FileKey { get; set; } = null!;
        public int ExpiresInSeconds { get; set; }
    }
}
