using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JitsiAppointmentApi.Core.Entities
{
    public class DoctorProfile : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string About { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Avatar { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Bio { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        public int Experience { get; set; }

        public bool IsActive { get; set; } = true;


    }
} 