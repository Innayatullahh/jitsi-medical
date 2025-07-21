using JitsiAppointmentApi.Application.DTOs;

namespace JitsiAppointmentApi.Application.Interfaces
{
    public interface ITimeSlotService
    {
        Task<CreateTimeSlotResponse> CreateTimeSlotAsync(CreateTimeSlotRequest request);
        Task<TimeSlotsByDayResponse> GetTimeSlotsByDayAsync(string date);
        Task<TimeSlotsByWeekResponse> GetTimeSlotsByWeekAsync(string startDate);
        Task<TimeSlotsByMonthResponse> GetTimeSlotsByMonthAsync(string month);
        Task<DeleteTimeSlotResponse> DeleteTimeSlotAsync(string id);
    }
} 