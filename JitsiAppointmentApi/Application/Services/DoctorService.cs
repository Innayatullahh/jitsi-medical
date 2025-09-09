using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;

namespace JitsiAppointmentApi.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<CreateDoctorResponse> CreateDoctorAsync(CreateDoctorRequest request)
        {
            // Check if doctor with same email already exists
            var existingDoctor = await _doctorRepository.GetByEmailAsync(request.Email);
            if (existingDoctor != null)
            {
                return new CreateDoctorResponse
                {
                    Status = "error",
                    Message = "A doctor with this email already exists"
                };
            }

            var doctor = new DoctorProfile
            {
                FullName = request.FullName,
                About = request.About,
                Address = request.Address,
                Avatar = request.Avatar,
                Bio = request.Bio,
                Email = request.Email,
                Phone = request.Phone,
                Experience = request.Experience
            };

            var createdDoctor = await _doctorRepository.AddAsync(doctor);

            return new CreateDoctorResponse
            {
                Status = "success",
                Message = "Doctor profile created successfully",
                Data = MapToDoctorResponse(createdDoctor)
            };
        }

        public async Task<DoctorResponse?> GetDoctorByIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            return doctor != null ? MapToDoctorResponse(doctor) : null;
        }

        public async Task<GetDoctorsResponse> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetActiveDoctorsAsync();
            var doctorResponses = doctors.Select(MapToDoctorResponse).ToList();

            return new GetDoctorsResponse
            {
                Status = "success",
                Data = doctorResponses
            };
        }

        public async Task<UpdateDoctorResponse> UpdateDoctorAsync(int id, UpdateDoctorRequest request)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                return new UpdateDoctorResponse
                {
                    Status = "error",
                    Message = "Doctor not found"
                };
            }
            
            // Check if email is being changed and if it already exists
            if (doctor.Email != request.Email)
            {
                var existingDoctor = await _doctorRepository.GetByEmailAsync(request.Email);
                if (existingDoctor != null && existingDoctor.Id != doctor.Id)
                {
                    return new UpdateDoctorResponse
                    {
                        Status = "error",
                        Message = "A doctor with this email already exists"
                    };
                }
            }

            // Update properties
            doctor.FullName = request.FullName;
            doctor.About = request.About;
            doctor.Address = request.Address;
            doctor.Avatar = request.Avatar;
            doctor.Bio = request.Bio;
            doctor.Email = request.Email;
            doctor.Phone = request.Phone;
            doctor.Experience = request.Experience;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _doctorRepository.UpdateAsync(doctor);

            return new UpdateDoctorResponse
            {
                Status = "success",
                Message = "Doctor profile updated successfully",
                Data = MapToDoctorResponse(doctor)
            };
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return false;

            await _doctorRepository.DeleteAsync(id);
            return true;
        }

        private static DoctorResponse MapToDoctorResponse(DoctorProfile doctor)
        {
            return new DoctorResponse
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                About = doctor.About,
                Address = doctor.Address,
                Avatar = doctor.Avatar,
                Bio = doctor.Bio,
                Email = doctor.Email,
                Phone = doctor.Phone,
                Experience = doctor.Experience,
                CreatedAt = doctor.CreatedAt,
                UpdatedAt = doctor.UpdatedAt
            };
        }
    }
} 