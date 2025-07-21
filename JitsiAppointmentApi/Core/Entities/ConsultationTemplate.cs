using System;
using System.ComponentModel.DataAnnotations;

namespace JitsiAppointmentApi.Core.Entities
{
    public class ConsultationTemplate : BaseEntity
    {
        public long Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Interval { get; set; }

        [Required]
        public string TimeRangesJson { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
} 