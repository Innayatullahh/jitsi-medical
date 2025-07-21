using System;

namespace JitsiAppointmentApi.Application.DTOs
{
    public class CreateMeetingRequest
    {
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
    }

    public class MeetingResponse
    {
        public int Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMeetingResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public MeetingResponse? Data { get; set; }
    }

    public class JoinLinkResponse
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string CustomJoinUrl { get; set; } = string.Empty;
    }
} 