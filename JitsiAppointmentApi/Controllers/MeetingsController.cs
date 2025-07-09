using JitsiAppointmentApi.Config;
using JitsiAppointmentApi.Data;
using JitsiAppointmentApi.Models;
using JitsiAppointmentApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JitsiAppointmentApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MeetingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JitsiJwtService _jitsiJwtService;
        private readonly JitsiOptions _jitsiOptions;

        public MeetingsController(AppDbContext context, JitsiJwtService jitsiJwtService, IOptions<JitsiOptions> jitsiOptions)
        {
            _context = context;
            _jitsiJwtService = jitsiJwtService;
            _jitsiOptions = jitsiOptions.Value;
        }

        // POST: api/meetings
        [HttpPost]
        public async Task<ActionResult<Meeting>> CreateMeeting([FromBody] Meeting meeting)
        {
            // Generate a unique Jitsi room name
            meeting.RoomName = $"jitsi-{Guid.NewGuid()}";
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMeeting), new { id = meeting.Id }, meeting);
        }

        // GET: api/meetings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> GetMeeting(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
                return NotFound();
            return meeting;
        }

        // GET: api/meetings/{id}/join-link
        [HttpGet("{id}/join-link")]
        public async Task<ActionResult<object>> GetJoinLink(int id, [FromQuery] string userName, [FromQuery] string userEmail)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
                return NotFound();

            // Generate JWT for this user and room
            var token = _jitsiJwtService.GenerateToken(meeting.RoomName, userName, userEmail);            
            //var directJoinUrl = $"{_jitsiOptions.ServerUrl}/{meeting.RoomName}?jwt={token}";
            var customJoinUrl = $"{_jitsiOptions.AppBaseUrl}/meeting.html?room={meeting.RoomName}&jwt={token}";

            return Ok(new
            {
                meeting.Id,
                meeting.DoctorName,
                meeting.PatientName,
                meeting.ScheduledAt,
                meeting.RoomName,
                //DirectJoinUrl = directJoinUrl,
                CustomJoinUrl = customJoinUrl
            });
        }
    }
}