namespace JitsiAppointmentApi.Application.DTOs
{
    public class DashboardResponseDTOs
    {
        public string Status { get; set; } = "success";
        public int TotalAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public List<AppointmentResponse> Data { get; set; } = new();
    }
}
