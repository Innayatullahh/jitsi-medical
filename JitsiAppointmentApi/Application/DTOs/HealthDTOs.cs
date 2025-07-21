using System;

namespace JitsiAppointmentApi.Application.DTOs
{
    public class HealthResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public HealthChecks? Checks { get; set; }
        public string? Error { get; set; }
    }

    public class HealthChecks
    {
        public string Database { get; set; } = string.Empty;
    }

    public class ReadyResponse
    {
        public string Status { get; set; } = string.Empty;
    }

    public class LiveResponse
    {
        public string Status { get; set; } = string.Empty;
    }
} 