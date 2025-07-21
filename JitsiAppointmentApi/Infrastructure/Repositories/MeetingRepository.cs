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
    }
} 