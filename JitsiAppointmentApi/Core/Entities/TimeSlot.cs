using System;
using System.ComponentModel.DataAnnotations;

namespace JitsiAppointmentApi.Core.Entities
{
    public class TimeSlot : BaseEntity
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string StartTime { get; set; } = string.Empty;

        [Required]
        public string EndTime { get; set; } = string.Empty;

        public bool IsVirtual { get; set; }

        public bool IsRecurring { get; set; }

        [Required]
        public string RecurringDaysJson { get; set; } = string.Empty;

        public DateTime? Date { get; set; }

        public bool IsBooked { get; set; }
    }
} 