using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class IGM
    {
        [Key]
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }

        // Computed property (not stored in DB)
        [NotMapped]
        public string FormattedNumber => $"{Number}/{Date.Year}";
        public string Vessel { get; set; } = string.Empty;
        public int PortId { get; set; }
        public Port? Port { get; set; }


        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        // Navigation
        public ICollection<PendingBL> PendingBLs { get; set; }
    }
}
