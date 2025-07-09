namespace JitsiAppointmentApi.Models
{
    public class Meeting
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string RoomName { get; set; } = string.Empty;
    }
}
