using System.ComponentModel.DataAnnotations;

namespace PPCMD.Models
{
    public class LC
    {
        [Key]
        public int Id { get; set; }
        public string? LCNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalQuantity { get; set; }  // Total BL Quantity

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        public ICollection<BL> BLs { get; set; } = new List<BL>();
        public ICollection<Payorder> Payorders { get; set; } = new List<Payorder>();
    }
}