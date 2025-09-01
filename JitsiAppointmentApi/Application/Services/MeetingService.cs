using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;
using JitsiAppointmentApi.Infrastructure.Config;
using JitsiAppointmentApi.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace JitsiAppointmentApi.Application.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly JitsiJwtService _jitsiJwtService;
        private readonly JitsiOptions _jitsiOptions;

        public MeetingService(IMeetingRepository meetingRepository, JitsiJwtService jitsiJwtService, IOptions<JitsiOptions> jitsiOptions)
        {
            _meetingRepository = meetingRepository;
            _jitsiJwtService = jitsiJwtService;
            _jitsiOptions = jitsiOptions.Value;
        }

        public async Task<CreateMeetingResponse> CreateMeetingAsync(CreateMeetingRequest request)
        {
            // Always generate a unique Jitsi room name (like old code)
            var roomName = $"jitsi-{Guid.NewGuid()}";

            var meeting = new Meeting
            {
                DoctorName = request.DoctorName,
                PatientName = request.PatientName,
                RoomName = roomName,
                ScheduledAt = DateTime.SpecifyKind(request.ScheduledAt, DateTimeKind.Utc)
            };

            var createdMeeting = await _meetingRepository.AddAsync(meeting);

            return new CreateMeetingResponse
            {
                Status = "success",
                Message = "Meeting created successfully",
                Data = MapToMeetingResponse(createdMeeting)
            };
        }

        public async Task<MeetingResponse?> GetMeetingByIdAsync(int id)
        {
            var meeting = await _meetingRepository.GetByIdAsync(id);
            return meeting != null ? MapToMeetingResponse(meeting) : null;
        }

        public async Task<JoinLinkResponse?> GetJoinLinkAsync(int id, string userName)
        {
            var meeting = await _meetingRepository.GetByIdAsync(id);
            if (meeting == null)
                return null;

            // Generate JWT for this user and room
            var token = _jitsiJwtService.GenerateToken(meeting.RoomName, userName, userName + "@example.com");
            var customJoinUrl = $"{_jitsiOptions.AppBaseUrl}/meeting.html?room={meeting.RoomName}&jwt={token}";

            return new JoinLinkResponse
            {
                Id = meeting.Id,
                RoomName = meeting.RoomName,
                CustomJoinUrl = customJoinUrl
            };
        }

        public async Task<IEnumerable<Meeting>> GetAllMeetingsAsync()
        {
            return await _meetingRepository.GetAllAsync();
        }

        private static MeetingResponse MapToMeetingResponse(Meeting meeting)
        {
            return new MeetingResponse
            {
                Id = meeting.Id,
                DoctorName = meeting.DoctorName,
                PatientName = meeting.PatientName,
                RoomName = meeting.RoomName,
                ScheduledAt = meeting.ScheduledAt,
                CreatedAt = meeting.CreatedAt,
                UpdatedAt = meeting.UpdatedAt
            };
        }
    }
} 