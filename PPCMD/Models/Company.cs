using System.ComponentModel.DataAnnotations;

namespace PPCMD.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string CompanyName { get; set; } = string.Empty; // Unique

        [Required]
        public int License { get; set; } // Unique

        [Required, MaxLength(50)]
        public required string NTN { get; set; }  // Unique

        [Required, MaxLength(50)]
        public required string STN { get; set; }  // Unique

        [Required, MaxLength(100)]
        public required string ProprietorName { get; set; }

        [Required, MaxLength(15)]
        public required string CNIC { get; set; } // Unique

        [Required, EmailAddress, MaxLength(100)]
        public required string Email { get; set; } // Unique

        [MaxLength(100)]
        public string? Website { get; set; }

        [MaxLength(20)]
        public required string Landline1 { get; set; }

        [MaxLength(20)]
        public string? Landline2 { get; set; }

        [MaxLength(20)]
        public required string Mobile { get; set; }

        [MaxLength(500)]
        public required string Address { get; set; }



        // Navigation property
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
