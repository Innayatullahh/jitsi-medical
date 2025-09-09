using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;
using JitsiAppointmentApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JitsiAppointmentApi.Infrastructure.Repositories
{
    public class MeetingRepository : BaseRepository<Meeting>, IMeetingRepository
    {
        public MeetingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Meeting?> GetByRoomNameAsync(string roomName)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.RoomName == roomName);
        }

        public async Task<bool> MeetingExistsAsync(string doctorName, DateTime scheduledAt)
        {
            return await _dbSet.AnyAsync(m => m.DoctorName.ToLower() == doctorName && m.ScheduledAt == scheduledAt);
        }

        public async Task<IEnumerable<Meeting>> SearchSortByStatusDoctorAsync(string patientName, string status, string sortBy)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(patientName))
            {
                query = query.Where(m => m.PatientName.ToLower() == patientName.ToLower());
            }

            var now = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(status))
            {
                switch (status?.ToLower())
                {
                    case "upcoming":
                        query = query.Where(m => m.ScheduledAt > now);
                        break;
                    case "completed":
                        query = query.Where(m => m.ScheduledAt < now);
                        break;
                    case "pending":
                        query = query.Where(m => m.ScheduledAt.Date == now);
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy?.ToLower())
                {
                    case "day":
                        query = query.Where(m => m.ScheduledAt.Date == now.Date)
                                     .OrderBy(m => m.ScheduledAt);
                        break;
                    case "week":
                        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                        var endOfWeek = startOfWeek.AddDays(7);
                        query = query.Where(m => m.ScheduledAt.Date >= startOfWeek && m.ScheduledAt.Date < endOfWeek)
                                     .OrderBy(m => m.ScheduledAt);
                        break;
                    case "month":
                        var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
                        var endOfMonth = DateTime.SpecifyKind(startOfMonth.AddMonths(1), DateTimeKind.Utc);
                        query = query.Where(m => m.ScheduledAt >= startOfMonth && m.ScheduledAt < endOfMonth)
                                     .OrderBy(m => m.ScheduledAt);
                        break;
                }
            }

            return await query.ToListAsync();
        }
    }
} 