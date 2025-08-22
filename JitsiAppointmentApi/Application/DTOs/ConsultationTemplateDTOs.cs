using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JitsiAppointmentApi.Application.DTOs
{
    public class CreateConsultationTemplateRequest
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Interval { get; set; }
        public List<TimeRange> TimeRanges { get; set; } = new List<TimeRange>();
    }

    // New DTOs for template endpoints matching client requirements
    public class CreateTemplateRequest
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Interval { get; set; }
        public List<TimeRange> TimeRanges { get; set; } = new List<TimeRange>();
    }

    public class TimeRange
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;
        
        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
        
        [JsonPropertyName("isBooked")]
        public bool IsBooked { get; set; } = false;
    }

    public class TimeRangeWithSlots : TimeRange
    {
        public List<string> TimeSlots { get; set; } = new List<string>();
    }

    public class TemplateResponse
    {
        public int Id { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Interval { get; set; }
        public List<TimeRange> TimeRanges { get; set; } = new List<TimeRange>();
    }

    public class CreateTemplateResponse
    {
        public string Message { get; set; } = string.Empty;
        public TemplateResponse Template { get; set; } = new TemplateResponse();
    }

    public class ConsultationTemplateResponse
    {
        public long Id { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Interval { get; set; }
        public List<TimeRangeWithSlots> TimeRanges { get; set; } = new List<TimeRangeWithSlots>();
    }

    public class CreateConsultationTemplateResponse
    {
        public string Message { get; set; } = string.Empty;
        public ConsultationTemplateResponse Template { get; set; } = new ConsultationTemplateResponse();
    }

    public class GetConsultationTemplatesResponse
    {
        public string Status { get; set; } = string.Empty;
        public List<ConsultationTemplateResponse> Data { get; set; } = new List<ConsultationTemplateResponse>();
    }
} 