using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        // --- Profile Info ---
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? FullName => $"{FirstName} {LastName}";

        [MaxLength(250)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Mobile { get; set; }

        [MaxLength(50)]
        public string CNIC { get; set; } = string.Empty;


        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [MaxLength(250)]
        public string? ProfilePictureUrl { get; set; }


        public bool IsActive { get; set; } = true;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

