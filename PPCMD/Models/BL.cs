using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class BL
    {
        [Key]
        public int Id { get; set; }


        public int? Containers { get; set; }
        public int? Size { get; set; }
        public string? Packages { get; set; }
        public decimal ExchangeRate { get; set; }
        public int? ShippingLineId { get; set; }
        public ShippingLine? ShippingLine { get; set; }
        public int? TerminalId { get; set; }
        public Terminal? Terminal { get; set; }
        public int? LoloId { get; set; }
        public Lolo? Lolo { get; set; }
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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        // Navigation
        //public ICollection<ST> STs { get; set; }
        public ICollection<HC> HomeConsumptions { get; set; } = new List<HC>();
        //public ICollection<IB> IBs { get; set; }
        //public ICollection<Exbond> Exbonds { get; set; }
        public ICollection<DutyCharge> DutyCharges { get; set; } = new List<DutyCharge>();
        public ICollection<Payorder> Payorders { get; set; } = new List<Payorder>();
    }
}