using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JitsiAppointmentApi.Application.DTOs
{
    public class CreateConsultationTemplateRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Interval { get; set; }
        public List<TimeRangeInput> TimeRanges { get; set; } = new();
    }

    public class TimeRange
    {
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public bool IsBooked { get; set; } = false;
    }

    public class TimeRangeInput
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;
        
        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
    }

    public class ConsultationTemplateResponse
    {
        public long Id { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Interval { get; set; }
        public List<TimeRangeWithSlots> TimeRanges { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimeRangeWithSlots
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;
        
        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
        
        [JsonPropertyName("timeSlots")]
        public List<TimeSlotDto> TimeSlots { get; set; } = new();
    }

    public class TimeSlotDto
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;
        
        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
        
        [JsonPropertyName("isBooked")]
        public bool IsBooked { get; set; } = false;
    }

    public class CreateConsultationTemplateResponse
    {
        public string Status { get; set; } = "success";
        public string Message { get; set; } = string.Empty;
        public ConsultationTemplateResponse Data { get; set; } = new();
    }

    public class GetConsultationTemplatesResponse
    {
        public string Status { get; set; } = "success";
        public List<ConsultationTemplateResponse> Data { get; set; } = new();
    }
}
