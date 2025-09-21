using PPCMD.Data;
using PPCMD.Models;
using Microsoft.EntityFrameworkCore;

namespace PPCMD.utils
{
    public class CompanySeeder
    {
        private readonly ApplicationDbContext _context;

        public CompanySeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDutyTypesForCompanyAsync(int companyId)
        {
            // Only add if not already seeded
            var existing = await _context.DutyTypes
                .Where(d => d.CompanyId == companyId)
                .AnyAsync();

            if (existing) return;

            var dutyTypes = new List<DutyType>
        {
            new DutyType { Name = "Custom Duty", Description = "Base import duty", CompanyId = companyId },
            new DutyType { Name = "Additional Custom Duty", Description = "Additional custom duty rate", CompanyId = companyId },
            new DutyType { Name = "Regulatory Duty", Description = "Regulatory duty imposed by government", CompanyId = companyId },
            new DutyType { Name = "Sales Tax", Description = "General sales tax", CompanyId = companyId },
            new DutyType { Name = "Additional Sales Tax", Description = "Extra sales tax on certain items", CompanyId = companyId },
            new DutyType { Name = "Income Tax", Description = "Advance income tax on imports", CompanyId = companyId },
            new DutyType { Name = "Additional Income Tax", Description = "Advance income tax on imports", CompanyId = companyId },
            new DutyType { Name = "PSQC", Description = "Pakistan Standards & Quality Control fee", CompanyId = companyId },
            new DutyType { Name = "Wharfage", Description = "Port wharfage charges", CompanyId = companyId }
        };

            _context.DutyTypes.AddRange(dutyTypes);
            await _context.SaveChangesAsync();
        }

        public async Task SeedClientTypesForCompanyAsync(int companyId)
        {
            // Only add if not already seeded
            var existing = await _context.ClientTypes
                .Where(ct => ct.CompanyId == companyId)
                .AnyAsync();
            if (existing) return;
            var clientTypes = new List<ClientType>
        {
            new ClientType { Name = "Commercial",  CompanyId = companyId },
            new ClientType { Name = "Industrial",  CompanyId = companyId },
            new ClientType { Name = "Government",  CompanyId = companyId },
            new ClientType { Name = "Individual",  CompanyId = companyId },
            new ClientType { Name = "Others",  CompanyId = companyId }
        };
            _context.ClientTypes.AddRange(clientTypes);
            await _context.SaveChangesAsync();
        }


        public async Task SeedClientsForCompanyAsync(int companyId)
        {
            // Get ClientTypes for this company
            var clientTypes = await _context.ClientTypes
                .Where(ct => ct.CompanyId == companyId)
                .ToListAsync();

            if (!clientTypes.Any()) return;

            // Only seed if no clients exist yet
            var existing = await _context.Clients
                .Where(c => c.CompanyId == companyId)
                .AnyAsync();
            if (existing) return;

            var rnd = new Random();

            var pakCities = new[] { "Karachi", "Lahore", "Islamabad", "Faisalabad", "Multan", "Quetta", "Peshawar" };

            var clients = new List<Client>();

            for (int i = 1; i <= 15; i++)
            {
                var type = (i % 2 == 0)
                    ? clientTypes.First(ct => ct.Name == "Commercial")
                    : clientTypes.First(ct => ct.Name == "Industrial");

                var client = new Client
                {
                    ClientName = $"Client {i} Pvt Ltd",
                    ContactPerson = $"Contact Person {i}",
                    Phone = $"021-{rnd.Next(3000000, 3999999)}",
                    Mobile = $"03{rnd.Next(0, 9)}-{rnd.Next(1000000, 9999999)}",
                    Address = $"{rnd.Next(10, 200)} {pakCities[rnd.Next(pakCities.Length)]}, Pakistan",
                    GST = $"07{i}{rnd.Next(1000, 9999)}",
                    NTN = $"NTN{i}{rnd.Next(10000, 99999)}",
                    ClientTypeId = type.Id,
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow
                };

                // Add emails
                int emailCount = rnd.Next(1, 4); // 1–3 emails
                for (int e = 1; e <= emailCount; e++)
                {
                    client.Emails.Add(new ClientEmail
                    {
                        Email = $"client{i}.contact{e}@example.com",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                clients.Add(client);
            }

            _context.Clients.AddRange(clients);
            await _context.SaveChangesAsync();
        }
    }

}
