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
    }

}
