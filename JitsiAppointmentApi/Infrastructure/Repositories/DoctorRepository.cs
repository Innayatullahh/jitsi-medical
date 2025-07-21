using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;
using JitsiAppointmentApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JitsiAppointmentApi.Infrastructure.Repositories
{
    public class DoctorRepository : BaseRepository<DoctorProfile>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<DoctorProfile?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.Email == email);
        }

        public async Task<IEnumerable<DoctorProfile>> GetActiveDoctorsAsync()
        {
            return await _dbSet.Where(d => d.IsActive).ToListAsync();
        }
    }
} 