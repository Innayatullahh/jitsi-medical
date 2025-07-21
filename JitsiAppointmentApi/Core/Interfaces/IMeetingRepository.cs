using JitsiAppointmentApi.Core.Entities;

namespace JitsiAppointmentApi.Core.Interfaces
{
    public interface IMeetingRepository : IRepository<Meeting>
    {
        Task<Meeting?> GetByRoomNameAsync(string roomName);
    }
} 