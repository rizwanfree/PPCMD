using System.ComponentModel.DataAnnotations;

namespace PPCMD.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        // Navigation property
        public ICollection<Client> Clients { get; set; } = new List<Client>();
    }
}
