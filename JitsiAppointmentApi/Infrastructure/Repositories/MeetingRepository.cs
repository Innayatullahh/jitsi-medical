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

        public async Task<IEnumerable<Meeting>> SearchSortByStatusDoctorAsync(string doctorName, string? status = "upcoming", string? sortBy = "day")
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(doctorName))
            {
                query = query.Where(m => m.DoctorName.ToLower() == doctorName.ToLower());
            }

            var now = DateTime.UtcNow;

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
                default:
                    query = query.Where(m => m.ScheduledAt.Date > now.Date); ;
                    break;
            }

            switch (sortBy?.ToLower())
            {
                case "week":
                    // Order by week number using ISO 8601 week calculation
                    query = query.OrderBy(m => System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        m.ScheduledAt,
                        System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Monday));
                    break;
                case "month":
                    query = query.OrderBy(m => m.ScheduledAt.Year).ThenBy(m => m.ScheduledAt.Month);
                    break;
                case "year":
                    query = query.OrderBy(m => m.ScheduledAt.Year);
                    break;
                default:
                    query = query.OrderBy(m => m.ScheduledAt.Day);
                    break;
            }

            return await query.ToListAsync();
        }
    }
} 