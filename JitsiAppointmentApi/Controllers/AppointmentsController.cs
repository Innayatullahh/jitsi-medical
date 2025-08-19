using JitsiAppointmentApi.Application.DTOs;
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
        private static readonly List<AppointmentResponse> appointments = new()
        {
            new AppointmentResponse
            {
                Id = "a1",
                Name = "John Smith",
                Date = "2025-06-23",
                StartTime = "10:30",
                EndTime = "11:30",
                Type = "Virtual",
                Status = "Completed"
            },
            new AppointmentResponse
            {
                Id = "a2",
                Name = "Alice Johnson",
                Date = "2025-06-24",
                StartTime = "12:00",
                EndTime = "13:00",
                Type = "In-Person",
                Status = "Upcoming"
            }
        };

        private static readonly List<AppointmentDetailResponse> appointmentDetails = new()
        {
            new AppointmentDetailResponse
            {
                Id = "a1",
                PatientName = "John Smith",
                Date = "2025-06-23",
                StartTime = "10:30 AM",
                EndTime = "11:30 AM",
                Type = "Virtual Appointment",
                AppoinmentsDetailsi = new List<string> { "Annual Checkup" },
                PatientHistory = new List<string>
                {
                    "Type 2 Diabetes (Diagnosed in 2018)",
                    "Penicillin",
                    "Recurring headaches, MRI advised",
                    "Appendectomy (2016)",
                    "Metformin 500mg twice daily",
                    "Father - Cardiac Issues, Mother - Diabetic",
                    "COVID-19 (2 doses, booster pending)"
                },
                PatientNotes = new List<string>
                {
                    "Mild chest discomfort after meals",
                    "Difficulty sleeping due to anxiety",
                    "Currently taking herbal supplements",
                    "Exercises 30 mins daily (walking)",
                    "Avoiding salt and sugar in diet",
                    "Recommended for specialist consult by family physician"
                }
            }
        };

        /// <summary>
        /// Get appointments
        /// </summary>
        /// <response code="200">Application is healthy</response>
        /// <response code="503">Application is unhealthy</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), 200)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> GetAppointments()
        {
            if (appointments.Count == 0)
                return StatusCode(503, new { status = "error", message = "No appointments available" });

            return Ok(new { status = "success", data = appointments });
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
            await Task.Delay(10);

            var detail = appointmentDetails.FirstOrDefault(a => a.Id == id);

            if (detail == null)
                return NotFound(new { status = "error", message = "Appointment not found" });

            return Ok(new { status = "success", data = detail });
        }
    }
}
