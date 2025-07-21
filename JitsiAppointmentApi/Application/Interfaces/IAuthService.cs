using JitsiAppointmentApi.Application.DTOs;

namespace JitsiAppointmentApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
} 