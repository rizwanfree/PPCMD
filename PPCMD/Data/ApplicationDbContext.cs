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
        public DbSet<Client> Clients { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemDuty> ItemDuties { get; set; }
        public DbSet<DutyType> DutyTypes { get; set; }


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

            // 🔧 Global Tenant Filters
            builder.Entity<ApplicationUser>()
                .HasQueryFilter(u => !EnableTenantFilter || !_currentCompanyId.HasValue || u.CompanyId == _currentCompanyId);

            builder.Entity<Employee>()
                .HasQueryFilter(e => !EnableTenantFilter || !_currentCompanyId.HasValue || e.CompanyId == _currentCompanyId);

            builder.Entity<Client>()
                .HasQueryFilter(c => !EnableTenantFilter || !_currentCompanyId.HasValue || c.CompanyId == _currentCompanyId);

            builder.Entity<Company>()
                .HasQueryFilter(c => !EnableTenantFilter || !_currentCompanyId.HasValue || c.Id == _currentCompanyId);


            builder.Entity<Item>()
                .HasQueryFilter(i => !EnableTenantFilter || !_currentCompanyId.HasValue || i.CompanyId == _currentCompanyId);
            builder.Entity<ItemDuty>()
                .HasQueryFilter(d => !EnableTenantFilter || !_currentCompanyId.HasValue || d.CompanyId == _currentCompanyId);
            builder.Entity<DutyType>()
                .HasQueryFilter(dt => !EnableTenantFilter || !_currentCompanyId.HasValue || dt.CompanyId == _currentCompanyId);
        }

        // Optional: expose a method to set current tenant at runtime
        public void SetTenant(int? companyId)
        {
            _currentCompanyId = companyId;
        }
    }
}
