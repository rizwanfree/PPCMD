using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class DutyCharge
    {
        [Key]
        public int Id { get; set; }

        // 🔗 Link to BL
        [ForeignKey(nameof(BL))]
        public int BLId { get; set; }
        public BL? BL { get; set; }

        // Base values for calculation
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportValue { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuranceValue { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal LadingCharges { get; set; }

        // 🔗 Link to DutyType
        [ForeignKey(nameof(DutyType))]
        public int DutyTypeId { get; set; }
        public DutyType DutyType { get; set; } = null!;

        // Applied Rate (snapshot from ItemDuty)
        [Column(TypeName = "decimal(10,4)")]
        public decimal Rate { get; set; }

        public bool IsPercentage { get; set; } = true;

        // Final duty amount (whole rupees)
        public int Amount { get; set; }

        // Multi-tenant
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
