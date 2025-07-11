using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JitsiAppointmentApi.Data;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Check database connectivity
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return StatusCode(503, new
                    {
                        status = "unhealthy",
                        timestamp = DateTime.UtcNow,
                        checks = new
                        {
                            database = "unhealthy"
                        }
                    });
                }

                return Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    checks = new
                    {
                        database = "healthy"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    checks = new
                    {
                        database = "unhealthy"
                    }
                });
            }
        }

        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            try
            {
                // Check if the application is ready to serve traffic
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return StatusCode(503, new { status = "not ready" });
                }

                return Ok(new { status = "ready" });
            }
            catch
            {
                return StatusCode(503, new { status = "not ready" });
            }
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            // Simple liveness check - just return OK if the application is running
            return Ok(new { status = "alive" });
        }
    }
} 