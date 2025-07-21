using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/health")]
    public class HealthController : ControllerBase
    {
        private readonly IHealthService _healthService;
        private readonly AppDbContext _context;

        public HealthController(IHealthService healthService, AppDbContext context)
        {
            _healthService = healthService;
            _context = context;
        }

        /// <summary>
        /// Get application health status
        /// </summary>
        /// <returns>Health status with database connectivity check</returns>
        /// <response code="200">Application is healthy</response>
        /// <response code="503">Application is unhealthy</response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthResponse), 200)]
        [ProducesResponseType(typeof(HealthResponse), 503)]
        public async Task<IActionResult> Get()
        {
            var health = await _healthService.GetHealthAsync();
            
            if (health.Status == "healthy")
            {
                return Ok(health);
            }
            
            return StatusCode(503, health);
        }

        /// <summary>
        /// Test database connection directly
        /// </summary>
        /// <returns>Database connection status</returns>
        [HttpGet("db-test")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                // Test if we can connect to the database
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (canConnect)
                {
                    // Test if we can execute a simple query
                    var doctorCount = await _context.DoctorProfiles.CountAsync();
                    
                    return Ok(new
                    {
                        Status = "success",
                        Message = "Database connection successful",
                        CanConnect = canConnect,
                        DoctorCount = doctorCount,
                        ConnectionString = _context.Database.GetConnectionString()?.Replace("Password=123456", "Password=***") // Hide password
                    });
                }
                else
                {
                    return StatusCode(503, new
                    {
                        Status = "error",
                        Message = "Cannot connect to database",
                        CanConnect = canConnect,
                        ConnectionString = _context.Database.GetConnectionString()?.Replace("Password=123456", "Password=***")
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "error",
                    Message = "Database connection failed",
                    Error = ex.Message,
                    ConnectionString = _context.Database.GetConnectionString()?.Replace("Password=123456", "Password=***")
                });
            }
        }

        /// <summary>
        /// Check if application is ready to serve traffic
        /// </summary>
        /// <returns>Readiness status</returns>
        /// <response code="200">Application is ready</response>
        /// <response code="503">Application is not ready</response>
        [HttpGet("ready")]
        [ProducesResponseType(typeof(ReadyResponse), 200)]
        [ProducesResponseType(typeof(ReadyResponse), 503)]
        public async Task<IActionResult> Ready()
        {
            var ready = await _healthService.GetReadyAsync();
            
            if (ready.Status == "ready")
            {
                return Ok(ready);
            }
            
            return StatusCode(503, ready);
        }

        /// <summary>
        /// Check if application is alive
        /// </summary>
        /// <returns>Liveness status</returns>
        /// <response code="200">Application is alive</response>
        [HttpGet("live")]
        [ProducesResponseType(typeof(LiveResponse), 200)]
        public IActionResult Live()
        {
            var live = _healthService.GetLive();
            return Ok(live);
        }
    }
} 