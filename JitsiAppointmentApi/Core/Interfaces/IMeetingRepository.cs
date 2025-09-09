using JitsiAppointmentApi.Core.Entities;

namespace JitsiAppointmentApi.Core.Interfaces
{
    public interface IMeetingRepository : IRepository<Meeting>
    {
        Task<Meeting?> GetByRoomNameAsync(string roomName);
        Task<bool> MeetingExistsAsync(string doctorName, DateTime scheduledAt);
        Task<IEnumerable<Meeting>> SearchSortByStatusDoctorAsync(string patientName, string status, string sortBy);
    }
} 