using System.Collections.Generic;

namespace JitsiAppointmentApi.Application.DTOs
{
    public class CreateTimeSlotRequest
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsVirtual { get; set; }
        public bool IsRecurring { get; set; }
        public List<string> RecurringDays { get; set; } = new List<string>();
        
        /// <summary>
        /// Date for non-recurring time slots. Required when IsRecurring is false, optional when IsRecurring is true.
        /// Format: yyyy-MM-dd
        /// </summary>
        public string? Date { get; set; } = null;
    }

    public class TimeSlotResponse
    {
        public string Id { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsVirtual { get; set; }
        public bool IsBooked { get; set; }
    }

    public class CreateTimeSlotResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public TimeSlotData? Data { get; set; }
    }

    public class TimeSlotData
    {
        public string Id { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsVirtual { get; set; }
        public bool IsRecurring { get; set; }
        public List<string> RecurringDays { get; set; } = new List<string>();
        
        /// <summary>
        /// Date for non-recurring time slots
        /// </summary>
        public string? Date { get; set; } = null;
    }

    public class TimeSlotsByDayResponse
    {
        public string Status { get; set; } = string.Empty;
        public List<TimeSlotResponse> Data { get; set; } = new List<TimeSlotResponse>();
    }

    public class TimeSlotsByWeekResponse
    {
        public string Status { get; set; } = string.Empty;
        public Dictionary<string, List<TimeSlotResponse>> Data { get; set; } = new Dictionary<string, List<TimeSlotResponse>>();
    }

    public class TimeSlotsByMonthResponse
    {
        public string Status { get; set; } = string.Empty;
        public Dictionary<string, List<TimeSlotResponse>> Data { get; set; } = new Dictionary<string, List<TimeSlotResponse>>();
    }

    public class DeleteTimeSlotResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
} 