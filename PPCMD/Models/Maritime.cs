namespace PPCMD.Models
{
    // Base class for all maritime-related business contacts
    public abstract class Maritime
    {
        public int Id { get; set; }                  // PK
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NTN { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }           // Tenant ID
        public Company? Company { get; set; }        // Navigation property

        // Audit & Soft Delete
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
    }

    // Terminal model
    public class Terminal : Maritime
    {        
    }

    // Shipping Line model
    public class ShippingLine : Maritime
    {        
    }

    // Lolo model
    public class Lolo : Maritime
    {
    }
}
