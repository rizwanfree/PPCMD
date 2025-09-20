using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{


    public class Item
    {
        [Key]
        public int ItemID { get; set; }

        [Required, MaxLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string HSCode { get; set; } = string.Empty;

        public int CompanyId { get; set; }  // 🔑 Multi-tenant support
        public Company? Company { get; set; }

        // Navigation Property
        public ICollection<ItemDuty> Duties { get; set; } = new List<ItemDuty>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class ItemDuty
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemID { get; set; }

        [ForeignKey(nameof(DutyType))]
        public int DutyTypeId { get; set; }
        public DutyType DutyType { get; set; } = null!;

        [Column(TypeName = "decimal(10,4)")] // precise for financial data
        public decimal Rate { get; set; }

        public bool IsPercentage { get; set; } = true; // 👈 Moved here

        public Item? Item { get; set; }
        public int CompanyId { get; set; }  // 🔑 Multi-tenant support
        public Company? Company { get; set; }
    }

    public class DutyType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // "Custom Duty", "Sales Tax", etc.
        public string? Description { get; set; }

        public int CompanyId { get; set; }  // 🔑 Multi-tenant support
        public Company? Company { get; set; }
    }

}
