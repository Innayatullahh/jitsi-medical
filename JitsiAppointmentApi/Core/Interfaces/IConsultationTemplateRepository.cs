using JitsiAppointmentApi.Core.Entities;

namespace JitsiAppointmentApi.Core.Interfaces
{
    public interface IConsultationTemplateRepository : IRepository<ConsultationTemplate>
    {
        Task<IEnumerable<ConsultationTemplate>> GetActiveTemplatesAsync();
    }
} 