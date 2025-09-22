using System;
using System.Collections.Generic;

namespace PPCMD.Models
{
    public class Client
    {
        public int Id { get; set; }   // 👈 PK

        // Basic Info
        public string ClientName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Company Identifiers
        public string GST { get; set; } = string.Empty;
        public string NTN { get; set; } = string.Empty;

        // Director Info
        public string? DirectorName { get; set; }
        public string? NIC { get; set; }
        public string? DirectorAddress { get; set; }

        // Location
        //public int CityId { get; set; }

        // 🔗 Relationship with ClientType
        public int ClientTypeId { get; set; }
        public ClientType? ClientType { get; set; }

        // Multi-Tenant Support
        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        // 🔗 Related Data
        public ICollection<ClientEmail> Emails { get; set; } = new List<ClientEmail>();

        // 📌 Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public class ClientEmail
    {
        public int Id { get; set; }   // 👈 PK        
        public string Email { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        // 📌 Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class ClientType
    {
        public int Id { get; set; }   // 👈 PK
        public string Name { get; set; } = string.Empty;    // e.g., "Commercial", "Industrial"

        // Multi-Tenant Support
        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public ICollection<Client> Clients { get; set; } = new List<Client>();

        // 📌 Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
