using JitsiAppointmentApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace JitsiAppointmentApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<DoctorProfile> DoctorProfiles { get; set; }
        public DbSet<ConsultationTemplate> ConsultationTemplates { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Meeting> Meetings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DoctorProfile
            modelBuilder.Entity<DoctorProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.About).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Avatar).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Bio).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Configure ConsultationTemplate
            modelBuilder.Entity<ConsultationTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.TimeRangesJson).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Configure TimeSlot
            modelBuilder.Entity<TimeSlot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(50);
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.RecurringDaysJson).IsRequired();
            });

            // Configure Meeting
            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DoctorName).IsRequired();
                entity.Property(e => e.PatientName).IsRequired();
                entity.Property(e => e.RoomName).IsRequired();
            });
        }
    }
} 