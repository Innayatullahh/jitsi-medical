using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JitsiAppointmentApi.Application.Services
{
    public class HealthService : IHealthService
    {
        private readonly AppDbContext _context;

        public HealthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HealthResponse> GetHealthAsync()
        {
            try
            {
                // Check database connectivity
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return new HealthResponse
                    {
                        Status = "unhealthy",
                        Timestamp = DateTime.UtcNow,
                        Checks = new HealthChecks
                        {
                            Database = "unhealthy"
                        }
                    };
                }

                return new HealthResponse
                {
                    Status = "healthy",
                    Timestamp = DateTime.UtcNow,
                    Checks = new HealthChecks
                    {
                        Database = "healthy"
                    }
                };
            }
            catch (Exception ex)
            {
                return new HealthResponse
                {
                    Status = "unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message,
                    Checks = new HealthChecks
                    {
                        Database = "unhealthy"
                    }
                };
            }
        }

        public async Task<ReadyResponse> GetReadyAsync()
        {
            try
            {
                // Check if the application is ready to serve traffic
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    return new ReadyResponse { Status = "not ready" };
                }

                return new ReadyResponse { Status = "ready" };
            }
            catch
            {
                return new ReadyResponse { Status = "not ready" };
            }
        }

        public LiveResponse GetLive()
        {
            // Simple liveness check - just return OK if the application is running
            return new LiveResponse { Status = "alive" };
        }
    }
} 