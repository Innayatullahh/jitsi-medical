using System.ComponentModel.DataAnnotations;

namespace JitsiAppointmentApi.Core.Entities
{
    public class Meeting : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string DoctorName { get; set; } = string.Empty;

        [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public string RoomName { get; set; } = string.Empty;

        public DateTime ScheduledAt { get; set; }
    }
} 