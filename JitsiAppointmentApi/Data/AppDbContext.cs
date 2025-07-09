using JitsiAppointmentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JitsiAppointmentApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Meeting> Meetings { get; set; }
    }
}