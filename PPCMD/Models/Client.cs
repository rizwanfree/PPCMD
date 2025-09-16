using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string ClientName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string ContactPerson { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string ContactPersonMobile { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Fax { get; set; }

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(City))]
        public int CityId { get; set; }

        [Required, MaxLength(50)]
        public string GST { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string NTN { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ClientType { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? DirectorName { get; set; }

        [MaxLength(20)]
        public string? NIC { get; set; }

        [MaxLength(500)]
        public string? DirectorAddress { get; set; }

        // Navigation property
        public City? City { get; set; }

        // Tenant binding
        [Required]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }
}