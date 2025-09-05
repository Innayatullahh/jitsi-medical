using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Core.Entities;

namespace JitsiAppointmentApi.Application.Interfaces
{
    public interface IMeetingService
    {
        Task<CreateMeetingResponse> CreateMeetingAsync(CreateMeetingRequest request);
        Task<MeetingResponse?> GetMeetingByIdAsync(int id);
        Task<JoinLinkResponse?> GetJoinLinkAsync(int id, string userName);
        Task<IEnumerable<Meeting>> GetAllMeetingsAsync();
        Task<IEnumerable<Meeting>> SearchMeetingsAsync(string doctorName, string? sortBy, string? status);
    }
} 