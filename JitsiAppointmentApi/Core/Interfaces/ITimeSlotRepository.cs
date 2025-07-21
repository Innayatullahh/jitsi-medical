using JitsiAppointmentApi.Core.Entities;

namespace JitsiAppointmentApi.Core.Interfaces
{
    public interface ITimeSlotRepository : IRepository<TimeSlot>
    {
        Task<IEnumerable<TimeSlot>> GetByDateAsync(DateTime date);
        Task<IEnumerable<TimeSlot>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TimeSlot>> GetRecurringByDayOfWeekAsync(string dayOfWeek);
    }
} 