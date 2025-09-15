using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PPCMD.Models;

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

            builder.Entity<Company>()
                .HasIndex(c => c.CompanyName).IsUnique();

            builder.Entity<Company>()
                .HasIndex(c => c.License).IsUnique();

            builder.Entity<Company>()
                .HasIndex(c => c.NTN).IsUnique();

            builder.Entity<Company>()
                .HasIndex(c => c.STN).IsUnique();

            builder.Entity<Company>()
                .HasIndex(c => c.CNIC).IsUnique();

            builder.Entity<Company>()
                .HasIndex(c => c.Email).IsUnique();

            builder.Entity<Company>()
                .HasIndex(c => c.Website).IsUnique();


            // ----- GLOBAL FILTER FOR MULTITENANT -----
            // Filter ApplicationUser by CompanyId
            builder.Entity<ApplicationUser>()
                   .HasQueryFilter(u => !EnableTenantFilter || !_currentCompanyId.HasValue || u.CompanyId == _currentCompanyId);

        }

        // Optional: expose a method to set current tenant at runtime
        public void SetTenant(int? companyId)
        {
            _currentCompanyId = companyId;
        }
    }
}
