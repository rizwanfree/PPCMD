using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class BL
    {
        [Key]
        public int Id { get; set; }
        public DateTime BLDate { get; set; }
        public string? CashRef { get; set; }
        public DateTime? CashDate { get; set; }
        public string? GDRef { get; set; }
        public DateTime? GDDate { get; set; }

        public int LCId { get; set; }
        [ForeignKey("LCId")]
        public LC? LC { get; set; }

        // ✅ One-to-One with PendingBL
        public int PendingBLId { get; set; }
        public PendingBL PendingBL { get; set; } = null!;

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        // Navigation
        //public ICollection<ST> STs { get; set; }
        public ICollection<HC> HomeConsumptions { get; set; }
        //public ICollection<IB> IBs { get; set; }
        //public ICollection<Exbond> Exbonds { get; set; }
        public ICollection<DutyCharge> DutyCharges { get; set; }
    }
}