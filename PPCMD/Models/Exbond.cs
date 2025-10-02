using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class Exbond
    {
        [Key]
        public int Id { get; set; }
        public int BLId { get; set; }
        public string? BLReference { get; set; }
        public decimal Quantity { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal InsuranceUSD { get; set; }
        public decimal InsurancePKR { get; set; }
        public decimal ImportValue { get; set; }


        [ForeignKey("BLId")]
        public BL? BL { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property
    }
}