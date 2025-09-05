using System;

namespace JitsiAppointmentApi.Application.DTOs
{
    public class CreateDoctorRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Experience { get; set; }
    }

    public class UpdateDoctorRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Experience { get; set; }
    }

    // New DTOs for doctor profile endpoints
    public class DoctorProfileResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int Experience { get; set; }
        public string Address { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }

    public class UpdateDoctorProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int Experience { get; set; }
        public string Address { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }

    public class CreateDoctorProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int Experience { get; set; }
        public string Address { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public IFormFile? ProfileImage { get; set; }
    }

    public class CreateDoctorProfileResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DoctorProfileResponse CreatedProfile { get; set; } = new DoctorProfileResponse();
    }

    public class UpdateDoctorProfileResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DoctorProfileResponse UpdatedProfile { get; set; } = new DoctorProfileResponse();
    }

    public class DoctorResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Experience { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateDoctorResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DoctorResponse? Data { get; set; }
    }

    public class UpdateDoctorResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DoctorResponse? Data { get; set; }
    }

    public class GetDoctorsResponse
    {
        public string Status { get; set; } = string.Empty;
        public List<DoctorResponse> Data { get; set; } = new List<DoctorResponse>();
    }
} 