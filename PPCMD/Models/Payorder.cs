using System.ComponentModel.DataAnnotations;

namespace PPCMD.Models
{
    public class Payorder
    {
        [Key]
        public int Id { get; set; }

        // Optional link to BL
        public int? BLId { get; set; }
        public BL? BL { get; set; }

        // Optional link to LC (for combined/bulk payorders)
        public int? LCId { get; set; }
        public LC? LC { get; set; }

        [Required, MaxLength(200)]
        public string Particular { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public string? Detail { get; set; }

        public int Order { get; set; }   // 👈 ordering field

        // Multi-tenant support
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
