using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/doctor-profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public ProfileController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        /// <summary>
        /// Create a new doctor profile
        /// </summary>
        /// <param name="request">Doctor profile information</param>
        /// <returns>Created doctor profile</returns>
        /// <response code="201">Doctor profile created successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateDoctorProfileResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CreateDoctorProfileResponse>> CreateDoctorProfile([FromForm] DoctorProfileRequest request)
        {
            try
            {
                string? imageUrl = null;
                if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Avatars");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfileImage.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.ProfileImage.CopyToAsync(stream);
                    }

                    imageUrl = $"/Avatars/{uniqueFileName}";
                }

                var createRequest = new CreateDoctorRequest
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Bio = request.Bio,
                    Experience = request.Experience,
                    Address = request.Address,
                    About = request.About,
                    Avatar = imageUrl ?? ""
                };
                
                var result = await _doctorService.CreateDoctorAsync(createRequest);
                
                if (result.Status == "error")
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
                
                var response = new CreateDoctorProfileResponse
                {
                    Success = true,
                    Message = "Doctor profile created successfully",
                    CreatedProfile = new DoctorProfileResponse
                    {
                        Id = result.Data!.Id,
                        FullName = result.Data.FullName,
                        Email = result.Data.Email,
                        Phone = result.Data.Phone,
                        Bio = result.Data.Bio,
                        Experience = result.Data.Experience,
                        Address = result.Data.Address,
                        About = result.Data.About,
                        Avatar = result.Data.Avatar
                    }
                };
                
                return CreatedAtAction(nameof(GetDoctorProfile), new { id = result.Data.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Fetch the logged-in doctor's current profile data
        /// </summary>
        /// <returns>Doctor profile data</returns>
        /// <response code="200">Returns the doctor profile</response>
        /// <response code="404">Doctor not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(DoctorProfileResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DoctorProfileResponse>> GetDoctorProfile()
        {
            try
            {
                // TODO: Get doctor ID from JWT token
                // For now, using a default ID - this should be extracted from the authenticated user
                var doctorId = 1; // This should come from JWT token
                
                var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                
                if (doctor == null)
                {
                    return NotFound("Doctor not found");
                }
                
                var response = new DoctorProfileResponse
                {
                    Id = doctor.Id,
                    FullName = doctor.FullName,
                    Email = doctor.Email,
                    Phone = doctor.Phone,
                    Bio = doctor.Bio,
                    Experience = doctor.Experience,
                    Address = doctor.Address,
                    About = doctor.About,
                    Avatar = doctor.Avatar
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the doctor's profile with data submitted from the form
        /// </summary>
        /// <param name="request">Updated doctor profile information</param>
        /// <returns>Updated doctor profile</returns>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Doctor not found</response>
        [HttpPut]
        [ProducesResponseType(typeof(UpdateDoctorProfileResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
            public async Task<ActionResult<UpdateDoctorProfileResponse>> UpdateDoctorProfile([FromForm] DoctorProfileRequest request)
            {
                try
                {
                    // TODO: Get doctor ID from JWT token
                    var doctorId = 1; // This should come from JWT token

                    string? imageUrl = string.Empty;

                    if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                    {
                        // Optionally: Get the current doctor to delete the old image
                        var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
                        if (doctor != null && !string.IsNullOrEmpty(doctor.Avatar))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doctor.Avatar.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Avatars");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.ProfileImage.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.ProfileImage.CopyToAsync(stream);
                        }

                        imageUrl = $"/Avatars/{uniqueFileName}";
                    }

                    var updateRequest = new UpdateDoctorRequest
                    {
                        FullName = request.FullName,
                        Email = request.Email,
                        Phone = request.Phone,
                        Bio = request.Bio,
                        Experience = request.Experience,
                        Address = request.Address,
                        About = request.About,
                        Avatar = imageUrl ?? ""
                    };

                    var result = await _doctorService.UpdateDoctorAsync(doctorId, updateRequest);

                    if (result.Status == "error")
                    {
                        if (result.Message.Contains("not found"))
                        {
                            return NotFound(new { success = false, message = "Doctor not found" });
                        }
                        return BadRequest(new { success = false, message = result.Message });
                    }

                    var response = new UpdateDoctorProfileResponse
                    {
                        Success = true,
                        Message = "Profile updated successfully",
                        UpdatedProfile = new DoctorProfileResponse
                        {
                            Id = result.Data!.Id,
                            FullName = result.Data.FullName,
                            Email = result.Data.Email,
                            Phone = result.Data.Phone,
                            Bio = result.Data.Bio,
                            Experience = result.Data.Experience,
                            Address = result.Data.Address,
                            About = result.Data.About,
                            Avatar = result.Data.Avatar
                        }
                    };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "Internal server error", message = ex.Message });
                }
            }
    }
}