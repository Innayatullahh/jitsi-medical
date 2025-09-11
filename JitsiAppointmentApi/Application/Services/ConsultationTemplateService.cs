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
            // Validate date range
            if (request.StartDate >= request.EndDate)
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

            // Convert time ranges to JSON
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            var filteredTimeRanges = request.TimeRanges
                .Where(x => !string.IsNullOrEmpty(x.Start) || !string.IsNullOrEmpty(x.End))
                .ToList();
            var timeRangesJson = System.Text.Json.JsonSerializer.Serialize(filteredTimeRanges, jsonOptions);

            // Check if timeRangesJson contains data
            if (string.IsNullOrWhiteSpace(timeRangesJson) || timeRangesJson == "[]" || filteredTimeRanges.Count == 0)
            {
                return new CreateConsultationTemplateResponse
                {
                    Message = "Time interval range cannot be empty"
                };
            }

            var template = new ConsultationTemplate
            {
                StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc),
                Interval = request.Interval,
                TimeRangesJson = timeRangesJson
            };

            var createdTemplate = await _templateRepository.AddAsync(template);

            return new CreateConsultationTemplateResponse
            {
                Message = "Template saved successfully.",
                Data = MapToConsultationTemplateResponse(createdTemplate)
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
            
            // Handle both old and new JSON formats
            List<TimeRangeInput> timeRangeInputs;
            try
            {
                // Try to parse as new format first
                timeRangeInputs = System.Text.Json.JsonSerializer.Deserialize<List<TimeRangeInput>>(template.TimeRangesJson, jsonOptions) ?? new List<TimeRangeInput>();
            }
            catch
            {
                // Fallback to old format if needed
                var oldTimeRanges = System.Text.Json.JsonSerializer.Deserialize<List<TimeRange>>(template.TimeRangesJson, jsonOptions) ?? new List<TimeRange>();
                timeRangeInputs = oldTimeRanges.Select(tr => new TimeRangeInput { Start = tr.Start, End = tr.End }).ToList();
            }
            
            // Convert to TimeRangeWithSlots and generate time slots
            var timeRangesWithSlots = timeRangeInputs.Select(tr => new TimeRangeWithSlots
            {
                Start = tr.Start,
                End = tr.End,
                TimeSlots = GenerateTimeSlots(tr.Start, tr.End, template.Interval)
            }).ToList();

            return new ConsultationTemplateResponse
            {
                Id = template.Id,
                StartDate = template.StartDate.ToString("yyyy-MM-dd"),
                EndDate = template.EndDate.ToString("yyyy-MM-dd"),
                Interval = template.Interval,
                TimeRanges = timeRangesWithSlots,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }

        private static List<TimeSlotDto> GenerateTimeSlots(string startTime, string endTime, int intervalMinutes)
        {
            var timeSlots = new List<TimeSlotDto>();
            
            if (TimeSpan.TryParse(startTime, out var start) && TimeSpan.TryParse(endTime, out var end))
            {
                var current = start;
                while (current < end)
                {
                    var slotEnd = current.Add(TimeSpan.FromMinutes(intervalMinutes));
                    if (slotEnd <= end)
                    {
                        timeSlots.Add(new TimeSlotDto 
                        { 
                            Start = FormatTime(current), 
                            End = FormatTime(slotEnd), 
                            IsBooked = false 
                        });
                    }
                    current = slotEnd;
                }
            }
            
            return timeSlots;
        }

        private static string FormatTime(TimeSpan time)
        {
            var hours = time.Hours;
            var minutes = time.Minutes;
            var ampm = hours >= 12 ? "PM" : "AM";
            
            if (hours == 0)
                hours = 12;
            else if (hours > 12)
                hours -= 12;
                
            return $"{hours}:{minutes:D2} {ampm}";
        }
    }
}
