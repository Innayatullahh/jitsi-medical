using JitsiAppointmentApi.Core.Entities;

namespace JitsiAppointmentApi.Core.Interfaces
{
    public interface IDoctorRepository : IRepository<DoctorProfile>
    {
        Task<DoctorProfile?> GetByEmailAsync(string email);
        Task<IEnumerable<DoctorProfile>> GetActiveDoctorsAsync();
    }
} 