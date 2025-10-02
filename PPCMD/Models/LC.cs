using System.ComponentModel.DataAnnotations;

namespace PPCMD.Models
{
    public class LC
    {
        [Key]
        public int Id { get; set; }
        public string? LCNumber { get; set; }
        public DateTime Date { get; set; }
        public int EntryType { get; set; }
        public decimal Quantity { get; set; }
        public decimal DeclaredValue { get; set; }
        public decimal AssessedValue { get; set; }
        public decimal ExchangeRate { get; set; }
        public int LandingCharges { get; set; }


        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        public ICollection<BL> BLs { get; set; }
    }
}