using JitsiAppointmentApi.Application.DTOs;

namespace JitsiAppointmentApi.Application.Interfaces
{
    public interface IMeetingService
    {
        Task<CreateMeetingResponse> CreateMeetingAsync(CreateMeetingRequest request);
        Task<MeetingResponse?> GetMeetingByIdAsync(int id);
        Task<JoinLinkResponse?> GetJoinLinkAsync(int id, string userName);
    }
} 