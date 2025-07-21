using JitsiAppointmentApi.Application.DTOs;

namespace JitsiAppointmentApi.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<CreateDoctorResponse> CreateDoctorAsync(CreateDoctorRequest request);
        Task<DoctorResponse?> GetDoctorByIdAsync(int id);
        Task<GetDoctorsResponse> GetAllDoctorsAsync();
        Task<UpdateDoctorResponse> UpdateDoctorAsync(int id, UpdateDoctorRequest request);
        Task<bool> DeleteDoctorAsync(int id);
    }
} 