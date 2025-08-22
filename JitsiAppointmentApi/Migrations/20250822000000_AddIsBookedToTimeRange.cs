using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JitsiAppointmentApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBookedToTimeRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Since TimeRanges are stored as JSON, we don't need to modify the database schema
            // The IsBooked field will be handled at the application level
            // This migration documents the addition of the IsBooked property to the TimeRange DTO
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No database changes to revert
        }
    }
} 