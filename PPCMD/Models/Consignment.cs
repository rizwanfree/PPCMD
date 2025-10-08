using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class Consignment
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string ConsignmentTitle { get; set; } = string.Empty; // Home Consumption, Into-Bond, Safe Transportation etc

        public bool IsSelf { get; set; } // Self-cleared or through agent?

        [Required]
        public int LCId { get; set; }

        [ForeignKey("LCId")]
        public LC? LC { get; set; }

        // Consider adding these for better tracking:
        public ConsignmentStatus Status { get; set; } = ConsignmentStatus.JobCreated; // Draft, Submitted, Approved, Completed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties you might need:
        public ICollection<BL> BLs { get; set; } = new List<BL>();
        public ICollection<Payorder> Payorders { get; set; } = new List<Payorder>();

        // Multi-Tenant Support
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
    }

    public enum ConsignmentStatus
    {
        JobCreated,
        BillPending,
        Completed,
        Cancelled
    }
}
