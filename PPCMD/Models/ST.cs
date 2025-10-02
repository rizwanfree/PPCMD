using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class ST
    {
        [Key]
        public int Id { get; set; }        
        public decimal WarehousingRate { get; set; }
        public decimal WarehousingAmount { get; set; }

        [ForeignKey("BLId")]
        public int BLId { get; set; }
        public BL? BL { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property
    }
}