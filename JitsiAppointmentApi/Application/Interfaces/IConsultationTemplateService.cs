using JitsiAppointmentApi.Application.DTOs;

namespace JitsiAppointmentApi.Application.Interfaces
{
    public interface IConsultationTemplateService
    {
        Task<CreateConsultationTemplateResponse> CreateTemplateAsync(CreateConsultationTemplateRequest request);
        Task<ConsultationTemplateResponse?> GetTemplateByIdAsync(int id);
        Task<GetConsultationTemplatesResponse> GetAllTemplatesAsync();
        Task<bool> DeleteTemplateAsync(int id);
    }
} 