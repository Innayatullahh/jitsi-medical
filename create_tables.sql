-- Create DoctorProfiles table
CREATE TABLE IF NOT EXISTS "DoctorProfiles" (
    "Id" SERIAL PRIMARY KEY,
    "FullName" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(100) NOT NULL,
    "Phone" VARCHAR(20) NOT NULL,
    "Address" VARCHAR(500) NOT NULL,
    "Bio" VARCHAR(200) NOT NULL,
    "About" VARCHAR(1000) NOT NULL,
    "Experience" INTEGER NOT NULL,
    "Avatar" VARCHAR(500) NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

-- Create ConsultationTemplates table
CREATE TABLE IF NOT EXISTS "ConsultationTemplates" (
    "Id" BIGSERIAL PRIMARY KEY,
    "StartDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "EndDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Interval" INTEGER NOT NULL,
    "TimeRangesJson" TEXT NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

-- Create Meetings table
CREATE TABLE IF NOT EXISTS "Meetings" (
    "Id" SERIAL PRIMARY KEY,
    "RoomName" TEXT NOT NULL,
    "DoctorName" TEXT NOT NULL,
    "PatientName" TEXT NOT NULL,
    "ScheduledAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

-- Create TimeSlots table
CREATE TABLE IF NOT EXISTS "TimeSlots" (
    "Id" VARCHAR(50) PRIMARY KEY,
    "StartTime" TEXT NOT NULL,
    "EndTime" TEXT NOT NULL,
    "IsVirtual" BOOLEAN NOT NULL,
    "IsRecurring" BOOLEAN NOT NULL,
    "RecurringDaysJson" TEXT NOT NULL,
    "Date" TIMESTAMP WITH TIME ZONE,
    "IsBooked" BOOLEAN NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE
);

-- Insert migration record
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20250718080000_InitialCreate', '9.0.6')
ON CONFLICT ("MigrationId") DO NOTHING; 