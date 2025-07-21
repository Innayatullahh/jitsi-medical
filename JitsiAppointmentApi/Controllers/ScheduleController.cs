using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/time-slots")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public ScheduleController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        /// <summary>
        /// Create a new time slot with time range, virtual toggle, and recurring options
        /// </summary>
        /// <param name="request">Time slot information</param>
        /// <returns>Created time slot</returns>
        /// <response code="200">Time slot created successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateTimeSlotResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CreateTimeSlotResponse>> CreateTimeSlot([FromBody] CreateTimeSlotRequest request)
        {
            var result = await _timeSlotService.CreateTimeSlotAsync(request);
            
            if (result.Status == "error")
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Get all time slots available for a specific day
        /// </summary>
        /// <param name="date">Date in various formats: yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy, yyyy/MM/dd, dd/MM/yyyy</param>
        /// <returns>List of time slots for the specified day</returns>
        /// <response code="200">Returns the time slots for the day</response>
        /// <response code="400">If the date format is invalid</response>
        [HttpGet("day")]
        [ProducesResponseType(typeof(TimeSlotsByDayResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TimeSlotsByDayResponse>> GetTimeSlotsByDay([FromQuery] string date)
        {
            var result = await _timeSlotService.GetTimeSlotsByDayAsync(date);
            
            if (result.Status == "error")
            {
                return BadRequest("Invalid date format. Supported formats: yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy, yyyy/MM/dd, dd/MM/yyyy");
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Returns all time slots grouped by day for a given week starting from startDate
        /// </summary>
        /// <param name="startDate">Start date in various formats: yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy, yyyy/MM/dd, dd/MM/yyyy</param>
        /// <returns>Time slots grouped by day for the week</returns>
        /// <response code="200">Returns the time slots for the week</response>
        /// <response code="400">If the date format is invalid</response>
        [HttpGet("week")]
        [ProducesResponseType(typeof(TimeSlotsByWeekResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TimeSlotsByWeekResponse>> GetTimeSlotsByWeek([FromQuery] string startDate)
        {
            var result = await _timeSlotService.GetTimeSlotsByWeekAsync(startDate);
            
            if (result.Status == "error")
            {
                return BadRequest("Invalid date format. Supported formats: yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy, yyyy/MM/dd, dd/MM/yyyy");
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Fetch time slots for an entire month, grouped by date
        /// </summary>
        /// <param name="month">Month in format yyyy-MM (e.g., 2025-07)</param>
        /// <returns>Time slots grouped by date for the month</returns>
        /// <response code="200">Returns the time slots for the month</response>
        /// <response code="400">If the month format is invalid</response>
        [HttpGet("month")]
        [ProducesResponseType(typeof(TimeSlotsByMonthResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TimeSlotsByMonthResponse>> GetTimeSlotsByMonth([FromQuery] string month)
        {
            var result = await _timeSlotService.GetTimeSlotsByMonthAsync(month);
            
            if (result.Status == "error")
            {
                return BadRequest("Invalid month format. Use YYYY-MM");
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Remove a time slot by ID
        /// </summary>
        /// <param name="id">Time slot ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">Time slot deleted successfully</response>
        /// <response code="404">Time slot not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(DeleteTimeSlotResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DeleteTimeSlotResponse>> DeleteTimeSlot(string id)
        {
            var result = await _timeSlotService.DeleteTimeSlotAsync(id);
            
            if (result.Status == "error")
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }
    }
} 