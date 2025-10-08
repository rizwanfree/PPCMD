using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class DutyCharge
    {
        [Key]
        public int Id { get; set; }

        // 🔗 Link to BLItem (CHANGED FROM BL)
        [ForeignKey(nameof(BLItem))]
        public int BLItemId { get; set; }
        public BLItem? BLItem { get; set; }

        // Link to DutyType
        [ForeignKey(nameof(DutyType))]
        public int DutyTypeId { get; set; }
        public DutyType DutyType { get; set; } = null!;

        // Applied Rate (snapshot from ItemDuty)
        [Column(TypeName = "decimal(10,4)")]
        public decimal Rate { get; set; }

        public bool IsPercentage { get; set; } = true;

        // Final duty amount (whole rupees)
        public int Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
