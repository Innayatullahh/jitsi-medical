using JitsiAppointmentApi.Core.Entities;

namespace JitsiAppointmentApi.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Check if we already have data
            if (context.DoctorProfiles.Any())
            {
                return; // Database already seeded
            }

            // Add default doctor profile
            var defaultDoctor = new DoctorProfile
            {
                FullName = "Dr. John Doe",
                Email = "doctor@example.com",
                Phone = "+1234567890",
                Bio = "General Practitioner",
                Experience = 5,
                Address = "123 Medical Center Dr",
                About = "Experienced general practitioner with expertise in primary care.",
                Avatar = "https://example.com/avatar.jpg",
                IsActive = true
            };

            context.DoctorProfiles.Add(defaultDoctor);
            await context.SaveChangesAsync();

            Console.WriteLine("Database seeded with default doctor profile.");
        }
    }
} 