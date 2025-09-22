using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PPCMD.Models;
using System.Reflection.Emit;

namespace PPCMD.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private int? _currentCompanyId;
        public bool EnableTenantFilter { get; set; } = true;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
                                    IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            SetCurrentTenant();
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }        

        // Item and Duty entities
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemDuty> ItemDuties { get; set; }
        public DbSet<DutyType> DutyTypes { get; set; }

        // Client Management entities
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientEmail> ClientEmails { get; set; }
        public DbSet<ClientType> ClientTypes { get; set; }

        // Maritime entities
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<ShippingLine> ShippingLines { get; set; }
        public DbSet<Lolo> Lolos { get; set; }
        public DbSet<City> Cities { get; set; }


        private void SetCurrentTenant()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                // Assuming NameIdentifier is IdentityUser.Id
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    // Note: Can't do async in constructor, must be done in OnModelCreating for static value or use shadow property
                    // We'll use a shadow property and global filter below
                }
            }
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔧 Company -> Employees
            builder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔧 Company -> Users
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔧 Employee -> ApplicationUser (One-to-One)
            builder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(u => u.Employee)
                .HasForeignKey<ApplicationUser>(u => u.EmployeeId) // FK lives in User table
                .OnDelete(DeleteBehavior.Restrict);


            // Item -> Company
            builder.Entity<Item>()
                .HasOne(i => i.Company)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ItemDuty -> Item
            builder.Entity<ItemDuty>()
                .HasOne(d => d.Item)
                .WithMany(i => i.Duties)
                .HasForeignKey(d => d.ItemID)
                .OnDelete(DeleteBehavior.Restrict);

            // ItemDuty -> DutyType
            builder.Entity<ItemDuty>()
                .HasOne(d => d.DutyType)
                .WithMany()
                .HasForeignKey(d => d.DutyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ItemDuty -> Company
            builder.Entity<ItemDuty>()
                .HasOne(d => d.Company)
                .WithMany()
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // DutyType -> Company
            builder.Entity<DutyType>()
                .HasOne(dt => dt.Company)
                .WithMany()
                .HasForeignKey(dt => dt.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);


            // Client -> Company
            builder.Entity<Client>()
                .HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ClientType -> Company
            builder.Entity<ClientType>()
                .HasOne(ct => ct.Company)
                .WithMany()
                .HasForeignKey(ct => ct.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Maritime -> Company & Tenant Filter
            builder.Entity<Terminal>()
                .HasOne(t => t.Company)
                .WithMany()
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ShippingLine>()
                .HasOne(s => s.Company)
                .WithMany()
                .HasForeignKey(s => s.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Lolo>()
                .HasOne(l => l.Company)
                .WithMany()
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            // 🔧 Global Tenant Filters
            builder.Entity<ApplicationUser>()
                .HasQueryFilter(u => !EnableTenantFilter || !_currentCompanyId.HasValue || u.CompanyId == _currentCompanyId);

            builder.Entity<Employee>()
                .HasQueryFilter(e => !EnableTenantFilter || !_currentCompanyId.HasValue || e.CompanyId == _currentCompanyId);

            builder.Entity<Client>()
                .HasQueryFilter(c => !EnableTenantFilter || !_currentCompanyId.HasValue || c.CompanyId == _currentCompanyId);
            
            builder.Entity<ClientType>()
                .HasQueryFilter(ct => !EnableTenantFilter || !_currentCompanyId.HasValue || ct.CompanyId == _currentCompanyId);

            //builder.Entity<ClientEmail>()
            //    .HasQueryFilter(ce => !EnableTenantFilter || !_currentCompanyId.HasValue || ce.CompanyId == _currentCompanyId);

            builder.Entity<Company>()
                .HasQueryFilter(c => !EnableTenantFilter || !_currentCompanyId.HasValue || c.Id == _currentCompanyId);


            builder.Entity<Item>()
                .HasQueryFilter(i => !EnableTenantFilter || !_currentCompanyId.HasValue || i.CompanyId == _currentCompanyId);
            builder.Entity<ItemDuty>()
                .HasQueryFilter(d => !EnableTenantFilter || !_currentCompanyId.HasValue || d.CompanyId == _currentCompanyId);
            builder.Entity<DutyType>()
                .HasQueryFilter(dt => !EnableTenantFilter || !_currentCompanyId.HasValue || dt.CompanyId == _currentCompanyId);


            builder.Entity<Terminal>()
                .HasQueryFilter(t => !EnableTenantFilter || !_currentCompanyId.HasValue || t.CompanyId == _currentCompanyId);

            builder.Entity<ShippingLine>()
                .HasQueryFilter(s => !EnableTenantFilter || !_currentCompanyId.HasValue || s.CompanyId == _currentCompanyId);

            builder.Entity<Lolo>()
                .HasQueryFilter(l => !EnableTenantFilter || !_currentCompanyId.HasValue || l.CompanyId == _currentCompanyId);
        }

        // Optional: expose a method to set current tenant at runtime
        public void SetTenant(int? companyId)
        {
            _currentCompanyId = companyId;
        }
    }
}
