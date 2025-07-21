using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;
using JitsiAppointmentApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JitsiAppointmentApi.Infrastructure.Repositories
{
    public class ConsultationTemplateRepository : BaseRepository<ConsultationTemplate>, IConsultationTemplateRepository
    {
        public ConsultationTemplateRepository(AppDbContext context) : base(context)
        {
        }



        public async Task<IEnumerable<ConsultationTemplate>> GetActiveTemplatesAsync()
        {
            return await _dbSet
                .Where(ct => ct.IsActive)
                .ToListAsync();
        }
    }
} 