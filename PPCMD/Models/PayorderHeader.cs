using System.ComponentModel.DataAnnotations;

namespace PPCMD.Models
{
    public class PayorderHeader
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Multi-tenant support
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
