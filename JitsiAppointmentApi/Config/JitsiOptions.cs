namespace JitsiAppointmentApi.Config
{
    public class JitsiOptions
    {
        public string ServerUrl { get; set; } = string.Empty;
        public string AppBaseUrl { get; set; } = string.Empty;
        public string JwtSecret { get; set; } = string.Empty;
        public string JwtIssuer { get; set; } = "jitsi";
        public string JwtAudience { get; set; } = "jitsi";
        public int JwtExpiryMinutes { get; set; } = 60;
    }
}