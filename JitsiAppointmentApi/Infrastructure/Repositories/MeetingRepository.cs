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
            return await _dbSet.AnyAsync(m => m.DoctorName == doctorName && m.ScheduledAt == scheduledAt);
        }

        public async Task<IEnumerable<Meeting>> SearchSortByStatusDoctorAsync(string doctorName, string status, string sortBy)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(m => m.DoctorName.ToLower() == doctorName.ToLower());
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
                        query = query.Where(m => m.ScheduledAt.Date == now.Date);
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
                        var startOfMonth = new DateTime(now.Year, now.Month, 1);
                        var endOfMonth = startOfMonth.AddMonths(1);
                        query = query.Where(m => m.ScheduledAt.Date >= startOfMonth.Date && m.ScheduledAt.Date < endOfMonth.Date)
                                     .OrderBy(m => m.ScheduledAt);
                        break;
                }
            }

            return await query.ToListAsync();
        }
    }
} 