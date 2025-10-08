using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class PendingBL
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
        public string BLNumber { get; set; } = string.Empty;
        public DateTime BLDate { get; set; }
        public decimal BLQuantity { get; set; }       // Total quantity of items related to this PendingBL
        public decimal AssignedQuantity { get; set; }
        public int IndexNumber { get; set; }
        public int JobNumber { get; set; }
        public DateTime JobDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int IGMId { get; set; }
        [ForeignKey("IGMId")]
        public IGM? IGM { get; set; }


        public BL? BL { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        // Navigation property for items in this BL
        public ICollection<BLItem> Items { get; set; } = new List<BLItem>();
    }

    public class BLItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(PendingBL))]
        public int PendingBLId { get; set; }
        public PendingBL PendingBL { get; set; } = null!;

        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }
        public Item? Item { get; set; }

        // Extra details per item in this BL
        public decimal Quantity { get; set; }
        public decimal DeclaredValue { get; set; }
        public decimal? AssessedValue { get; set; }
        public decimal? InvoiceValue { get; set; }
        public decimal? InvoiceValuePKR { get; set; }
        public decimal? InsuranceUSD { get; set; }
        public decimal? InsurancePKR { get; set; }
        public decimal? LandingCharges { get; set; }
        public decimal? AssessableValue { get; set; }

        public ICollection<DutyCharge> DutyCharges { get; set; } = new List<DutyCharge>();

        // Multi-tenant
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}