using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;
using JitsiAppointmentApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JitsiAppointmentApi.Infrastructure.Repositories
{
    public class TimeSlotRepository : BaseRepository<TimeSlot>, ITimeSlotRepository
    {
        public TimeSlotRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TimeSlot>> GetByDateAsync(DateTime date)
        {
            return await _dbSet
                .Where(ts => !ts.IsRecurring && ts.Date.HasValue && ts.Date.Value.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(ts => !ts.IsRecurring && ts.Date.HasValue && 
                            ts.Date.Value.Date >= startDate.Date && 
                            ts.Date.Value.Date <= endDate.Date)
                .ToListAsync();
        }



        public async Task<IEnumerable<TimeSlot>> GetRecurringByDayOfWeekAsync(string dayOfWeek)
        {
            var normalizedDay = dayOfWeek.ToLower();
            return await _dbSet
                .Where(ts => ts.IsRecurring && ts.RecurringDaysJson.ToLower().Contains(normalizedDay))
                .ToListAsync();
        }
    }
} 