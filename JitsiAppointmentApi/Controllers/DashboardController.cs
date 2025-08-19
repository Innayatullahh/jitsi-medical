using JitsiAppointmentApi.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        /// <summary>
        /// Retrieves dashboard metrics and a list of recent appointments.
        /// </summary>
        /// <returns>Returns a DashboardResponseDTOs object containing appointment statistics and details.</returns>

        [HttpGet]
        [ProducesResponseType(typeof(DashboardResponseDTOs), 200)]
        [ProducesResponseType(typeof(DashboardResponseDTOs), 503)]
        public async Task<IActionResult> GetDashboard()
        {
            var response = new DashboardResponseDTOs
            {
                TotalAppointments = 200,
                TodayAppointments = 20, 
                UpcomingAppointments = 15,
                CompletedAppointments = 150,
                Data = new List<AppointmentResponse>
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
                    },
                    new AppointmentResponse
                    {
                        Id = "a3",
                        Name = "John Doe",
                        Date = "2025-07-04",
                        StartTime = "12:00",
                        EndTime = "13:00",
                        Type = "In-Person",
                        Status = "Upcoming"
                    }
                }
            };

            if (response.Data.Count == 0)
                return StatusCode(503, response);

            return Ok(response);
        }
    }
}

