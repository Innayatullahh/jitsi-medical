using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;

namespace JitsiAppointmentApi.Application.Services
{
    public class ConsultationTemplateService : IConsultationTemplateService
    {
        private readonly IConsultationTemplateRepository _templateRepository;

        public ConsultationTemplateService(IConsultationTemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<CreateConsultationTemplateResponse> CreateTemplateAsync(CreateConsultationTemplateRequest request)
        {
            // Parse dates
            if (!DateTime.TryParse(request.StartDate, out var startDate) || !DateTime.TryParse(request.EndDate, out var endDate))
            {
                return new CreateConsultationTemplateResponse
                {
                    Message = "Invalid date format. Use YYYY-MM-DD format."
                };
            }

            // Validate date range
            if (startDate >= endDate)
            {
                return new CreateConsultationTemplateResponse
                {
                    Message = "End date must be after start date"
                };
            }

            // Validate interval is positive
            if (request.Interval <= 0)
            {
                return new CreateConsultationTemplateResponse
                {
                    Message = "Interval must be greater than 0"
                };
            }



            // Convert time ranges to JSON (allow empty time ranges)
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            var timeRangesJson = System.Text.Json.JsonSerializer.Serialize(request.TimeRanges ?? new List<TimeRange>(), jsonOptions);

            var template = new ConsultationTemplate
            {
                StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                Interval = request.Interval,
                TimeRangesJson = timeRangesJson
            };

            var createdTemplate = await _templateRepository.AddAsync(template);

            return new CreateConsultationTemplateResponse
            {
                Message = "Template saved successfully.",
                Template = MapToConsultationTemplateResponse(createdTemplate)
            };
        }

        public async Task<ConsultationTemplateResponse?> GetTemplateByIdAsync(int id)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            return template != null ? MapToConsultationTemplateResponse(template) : null;
        }

        public async Task<GetConsultationTemplatesResponse> GetAllTemplatesAsync()
        {
            var templates = await _templateRepository.GetActiveTemplatesAsync();
            var templateResponses = templates.Select(MapToConsultationTemplateResponse).ToList();

            return new GetConsultationTemplatesResponse
            {
                Status = "success",
                Data = templateResponses
            };
        }

        public async Task<bool> DeleteTemplateAsync(int id)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
                return false;

            await _templateRepository.DeleteAsync(id);
            return true;
        }

        private static ConsultationTemplateResponse MapToConsultationTemplateResponse(ConsultationTemplate template)
        {
            // Parse time ranges from JSON
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            var timeRanges = System.Text.Json.JsonSerializer.Deserialize<List<TimeRange>>(template.TimeRangesJson, jsonOptions) ?? new List<TimeRange>();
            
            // Convert to TimeRangeWithSlots and generate time slots
            var timeRangesWithSlots = timeRanges.Select(tr => new TimeRangeWithSlots
            {
                Start = tr.Start,
                End = tr.End,
                IsBooked = tr.IsBooked,
                TimeSlots = GenerateTimeSlots(tr.Start, tr.End, template.Interval)
            }).ToList();

            return new ConsultationTemplateResponse
            {
                Id = template.Id,
                StartDate = template.StartDate.ToString("yyyy-MM-dd"),
                EndDate = template.EndDate.ToString("yyyy-MM-dd"),
                Interval = template.Interval,
                TimeRanges = timeRangesWithSlots
            };
        }

        private static List<string> GenerateTimeSlots(string startTime, string endTime, int intervalMinutes)
        {
            var timeSlots = new List<string>();
            
            if (TimeSpan.TryParse(startTime, out var start) && TimeSpan.TryParse(endTime, out var end))
            {
                var current = start;
                while (current < end)
                {
                    timeSlots.Add(current.ToString(@"hh\:mm"));
                    current = current.Add(TimeSpan.FromMinutes(intervalMinutes));
                }
            }
            
            return timeSlots;
        }
    }
} 