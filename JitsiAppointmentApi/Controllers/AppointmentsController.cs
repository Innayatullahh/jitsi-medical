using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/appointments")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMeetingService _meetingService;

        public AppointmentsController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        /// <summary>
        /// Get appointments with pagination support
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 50)</param>
        /// <response code="200">Returns paginated appointments successfully</response>
        /// <response code="400">Invalid pagination parameters</response>
        /// <response code="503">No appointments available</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedAppointmentsResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> GetAppointments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validate pagination parameters
            if (page < 1)
            {
                return BadRequest(new { status = "error", message = "Page number must be greater than 0" });
            }

            if (pageSize < 1 || pageSize > 50)
            {
                return BadRequest(new { status = "error", message = "Page size must be between 1 and 50" });
            }

            try
            {
                // Get all meetings from database
                var allMeetings = await _meetingService.GetAllMeetingsAsync();
                
                if (allMeetings == null || !allMeetings.Any())
                {
                    return StatusCode(503, new { status = "error", message = "No appointments available" });
                }

                // Convert meetings to appointment responses
                var appointments = allMeetings.Select(m => new AppointmentResponse
                {
                    Id = m.Id.ToString(),
                    Name = m.PatientName,
                    Date = m.ScheduledAt.ToString("yyyy-MM-dd"),
                    StartTime = m.ScheduledAt.ToString("HH:mm"),
                    EndTime = m.ScheduledAt.AddHours(1).ToString("HH:mm"), // Assuming 1-hour appointments
                    Type = "Virtual", // Default to virtual for now
                    Status = m.ScheduledAt > DateTime.UtcNow ? "Upcoming" : "Completed"
                }).OrderByDescending(a => a.Date).ToList();

                // Calculate pagination
                var totalCount = appointments.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var skip = (page - 1) * pageSize;

                // Get paginated data
                var paginatedAppointments = appointments
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();

                // Create pagination metadata
                var pagination = new PaginationMetadata
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasNext = page < totalPages,
                    HasPrevious = page > 1
                };

                var response = new PaginatedAppointmentsResponse
                {
                    Status = "success",
                    Data = paginatedAppointments,
                    Pagination = pagination
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Get the details of a specific appointment by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the appointment.</param>
        /// <response code="200">Returns the appointment details successfully.</response>
        /// <response code="404">Appointment with the specified ID was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppointmentDetailResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAppointmentDetail(string id)
        {
            try
            {
                if (!int.TryParse(id, out int meetingId))
                {
                    return BadRequest(new { status = "error", message = "Invalid appointment ID format" });
                }

                var meeting = await _meetingService.GetMeetingByIdAsync(meetingId);

                if (meeting == null)
                {
                    return NotFound(new { status = "error", message = "Appointment not found" });
                }

                var detail = new AppointmentDetailResponse
                {
                    Id = meeting.Id.ToString(),
                    PatientName = meeting.PatientName,
                    Date = meeting.ScheduledAt.ToString("yyyy-MM-dd"),
                    StartTime = meeting.ScheduledAt.ToString("HH:mm"),
                    EndTime = meeting.ScheduledAt.AddHours(1).ToString("HH:mm"),
                    Type = "Virtual Appointment",
                    AppoinmentsDetailsi = new List<string> { "General Consultation" },
                    PatientHistory = new List<string>
                    {
                        "No previous medical history available"
                    },
                    PatientNotes = new List<string>
                    {
                        "Appointment scheduled via system"
                    }
                };

                return Ok(new { status = "success", data = detail });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Internal server error", details = ex.Message });
            }
        }
    }
}
