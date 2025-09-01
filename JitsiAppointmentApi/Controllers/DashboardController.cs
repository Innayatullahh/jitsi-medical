using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
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
        private readonly IMeetingService _meetingService;

        public DashboardController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        /// <summary>
        /// Retrieves dashboard metrics and the latest 15 recent appointments.
        /// </summary>
        /// <returns>Returns a DashboardResponseDTOs object containing appointment statistics and the latest 15 appointment details.</returns>

        [HttpGet]
        [ProducesResponseType(typeof(DashboardResponseDTOs), 200)]
        [ProducesResponseType(typeof(DashboardResponseDTOs), 503)]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                // Get all meetings from database
                var allMeetings = await _meetingService.GetAllMeetingsAsync();
                
                if (allMeetings == null)
                {
                    allMeetings = new List<Core.Entities.Meeting>();
                }

                var meetingsList = allMeetings.ToList();

                // Calculate dashboard metrics
                var totalAppointments = meetingsList.Count;
                var todayAppointments = meetingsList.Count(m => m.ScheduledAt.Date == DateTime.UtcNow.Date);
                var upcomingAppointments = meetingsList.Count(m => m.ScheduledAt > DateTime.UtcNow);
                var completedAppointments = meetingsList.Count(m => m.ScheduledAt <= DateTime.UtcNow);

                // Get latest 15 appointments
                var recentAppointments = meetingsList
                    .OrderByDescending(m => m.ScheduledAt)
                    .Take(15)
                    .Select(m => new AppointmentResponse
                    {
                        Id = m.Id.ToString(),
                        Name = m.PatientName,
                        Date = m.ScheduledAt.ToString("yyyy-MM-dd"),
                        StartTime = m.ScheduledAt.ToString("HH:mm"),
                        EndTime = m.ScheduledAt.AddHours(1).ToString("HH:mm"),
                        Type = "Virtual",
                        Status = m.ScheduledAt > DateTime.UtcNow ? "Upcoming" : "Completed"
                    })
                    .ToList();

                var response = new DashboardResponseDTOs
                {
                    TotalAppointments = totalAppointments,
                    TodayAppointments = todayAppointments,
                    UpcomingAppointments = upcomingAppointments,
                    CompletedAppointments = completedAppointments,
                    Data = recentAppointments
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(503, new DashboardResponseDTOs
                {
                    TotalAppointments = 0,
                    TodayAppointments = 0,
                    UpcomingAppointments = 0,
                    CompletedAppointments = 0,
                    Data = new List<AppointmentResponse>()
                });
            }
        }
    }
}

