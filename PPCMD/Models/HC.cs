using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class HC
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("BLId")]
        public int BLId { get; set; }
        public BL? BL { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property
    }
}