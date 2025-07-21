using JitsiAppointmentApi.Application.DTOs;

namespace JitsiAppointmentApi.Application.Interfaces
{
    public interface IHealthService
    {
        Task<HealthResponse> GetHealthAsync();
        Task<ReadyResponse> GetReadyAsync();
        LiveResponse GetLive();
    }
} 