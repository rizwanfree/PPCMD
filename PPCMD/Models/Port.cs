using System.ComponentModel.DataAnnotations;

namespace PPCMD.Models
{
    public class Port
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        // Audit & Soft Delete
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
