using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/meetings")]
    [Authorize]
    public class MeetingsController : ControllerBase
    {
        private readonly IMeetingService _meetingService;

        public MeetingsController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        /// <summary>
        /// Create a new meeting
        /// </summary>
        /// <param name="request">Meeting information</param>
        /// <returns>Created meeting</returns>
        /// <response code="201">Meeting created successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateMeetingResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CreateMeetingResponse>> CreateMeeting([FromBody] CreateMeetingRequest request)
        {
            var result = await _meetingService.CreateMeetingAsync(request);
            
            if (result.Status == "error")
            {
                return BadRequest(result);
            }
            
            return CreatedAtAction(nameof(GetMeeting), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Get a meeting by ID
        /// </summary>
        /// <param name="id">Meeting ID</param>
        /// <returns>Meeting details</returns>
        /// <response code="200">Returns the meeting</response>
        /// <response code="404">Meeting not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MeetingResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MeetingResponse>> GetMeeting(int id)
        {
            var meeting = await _meetingService.GetMeetingByIdAsync(id);
            
            if (meeting == null)
            {
                return NotFound("Meeting not found");
            }
            
            return Ok(meeting);
        }

        /// <summary>
        /// Get join link for a meeting
        /// </summary>
        /// <param name="id">Meeting ID</param>
        /// <param name="userName">User name</param>
        /// <returns>Join link information</returns>
        /// <response code="200">Returns the join link</response>
        /// <response code="404">Meeting not found</response>
        [HttpGet("{id}/join-link")]
        [ProducesResponseType(typeof(JoinLinkResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<JoinLinkResponse>> GetJoinLink(int id, [FromQuery] string userName)
        {
            var joinLink = await _meetingService.GetJoinLinkAsync(id, userName);
            
            if (joinLink == null)
            {
                return NotFound("Meeting not found");
            }
            
            return Ok(joinLink);
        }

        /// <summary>
        /// Search meeting based on doctor name and status
        /// </summary>
        /// <param name="doctorName">Doctor Name</param>
        /// <param name="status">Status</param>
        /// <param name="sortBy">Sort by </param>
        /// <returns>Meetings details</returns>
        /// <response code="200">Returns the join link</response>
        /// <response code="404">Meeting not found</response>

        [HttpGet("{doctorName}/search-meetings")]
        [ProducesResponseType(typeof(IEnumerable<MeetingResponse>), 200)]
        public async Task<ActionResult<IEnumerable<MeetingResponse>>> SearchMeetings(string doctorName, [FromQuery] string? sortBy, [FromQuery] string? status)
        {
            // Call the new service method that queries the DB with filters and sorting
            var meetings = await _meetingService.SearchMeetingsAsync(doctorName, sortBy, status);

            if (meetings == null)
            {
                return NotFound("Meeting not found");
            }

            var result = meetings.Select(m => new MeetingResponse
            {
                Id = m.Id,
                DoctorName = m.DoctorName,
                PatientName = m.PatientName,
                RoomName = m.RoomName,
                ScheduledAt = m.ScheduledAt,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            });

            return Ok(result);
        }
    }
}