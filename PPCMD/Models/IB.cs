using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class IB
    {
        [Key]
        public int Id { get; set; }        
        public decimal WarehousingRate { get; set; }
        public decimal WarehousingAmount { get; set; }
        public decimal ExciseRate { get; set; }
        public decimal ExciseAmount { get; set; }
        public string? GDRef { get; set; }
        public DateTime? GDDate { get; set; }
        public string? CashRef { get; set; }
        public DateTime? CashDate { get; set; }
        public decimal RemainingQuantity { get; set; }

        [ForeignKey("BLId")]
        public int BLId { get; set; }
        public BL BL { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property
    }
}