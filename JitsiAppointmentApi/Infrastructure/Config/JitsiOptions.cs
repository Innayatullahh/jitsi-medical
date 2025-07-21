namespace JitsiAppointmentApi.Infrastructure.Config
{
    public class JitsiOptions
    {
        public string AppId { get; set; } = string.Empty;
        public string AppSecret { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string AppBaseUrl { get; set; } = string.Empty;
        
        // JWT-related properties
        public int JwtExpiryMinutes { get; set; } = 60;
        public string JwtAudience { get; set; } = string.Empty;
        public string JwtIssuer { get; set; } = string.Empty;
        public string ServerUrl { get; set; } = string.Empty;
        public string JwtSecret { get; set; } = string.Empty;
    }
} 