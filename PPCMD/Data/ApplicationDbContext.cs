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
        public DbSet<Port> Ports { get; set; }


        // Consignment Entities
        public DbSet<LC> LCs { get; set; }
        public DbSet<BL> BLs { get; set; }
        public DbSet<PendingBL> PendingBLs { get; set; }
        public DbSet<BLItem> BLItems { get; set; }
        public DbSet<IGM> IGMs { get; set; }
        public DbSet<DutyCharge> DutyCharges { get; set; }
        public DbSet<HC> HCs { get; set; }
        public DbSet<Consignment> Consignments { get; set; }


        // Payorder Entity
        public DbSet<Payorder> Payorders { get; set; }
        public DbSet<PayorderHeader> PayorderHeaders { get; set; }


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
                .OnDelete(DeleteBehavior.Restrict);

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
                .OnDelete(DeleteBehavior.Restrict);

            // DutyType -> Company
            builder.Entity<DutyType>()
                .HasOne(dt => dt.Company)
                .WithMany()
                .HasForeignKey(dt => dt.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            // Client -> Company
            builder.Entity<Client>()
                .HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

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

            // City -> Company
            builder.Entity<City>()
                .HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Port -> Company
            builder.Entity<Port>()
                .HasOne(p => p.Company)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);




            // BL -> Company
            builder.Entity<BL>()
                .HasOne(b => b.Company)
                .WithMany()
                .HasForeignKey(b => b.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BL>()
                .HasOne(b => b.PendingBL)
                .WithOne(pb => pb.BL)
                .HasForeignKey<BL>(b => b.PendingBLId)
                .OnDelete(DeleteBehavior.Restrict); // no cascade loops


            // PendingBL -> IGM
            builder.Entity<PendingBL>()
                .HasOne(pb => pb.IGM)
                .WithMany()
                .HasForeignKey(pb => pb.IGMId)
                .OnDelete(DeleteBehavior.Restrict);

            // BLItem -> PendingBL
            builder.Entity<BLItem>()
                .HasOne(bi => bi.PendingBL)
                .WithMany(pb => pb.Items)
                .HasForeignKey(bi => bi.PendingBLId)
                .OnDelete(DeleteBehavior.Restrict);

            // BLItem -> Item
            builder.Entity<BLItem>()
                .HasOne(bi => bi.Item)
                .WithMany()
                .HasForeignKey(bi => bi.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // DutyCharge -> BLItem (CHANGED FROM BL)
            builder.Entity<DutyCharge>()
                .HasOne(dc => dc.BLItem)
                .WithMany() // If BLItem has collection of DutyCharges, use: .WithMany(bli => bli.DutyCharges)
                .HasForeignKey(dc => dc.BLItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // DutyCharge -> DutyType
            builder.Entity<DutyCharge>()
                .HasOne(dc => dc.DutyType)
                .WithMany() // If DutyType has collection of DutyCharges, use: .WithMany(dt => dt.DutyCharges)
                .HasForeignKey(dc => dc.DutyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // HC -> BL (one BL can have many HomeConsumptions)
            builder.Entity<HC>()
                .HasOne(h => h.BL)
                .WithMany(b => b.HomeConsumptions)   // ensure BL has ICollection<HC> HomeConsumptions
                .HasForeignKey(h => h.BLId)
                .OnDelete(DeleteBehavior.Restrict); // use Restrict to avoid accidental data loss

            // HC -> Company (tenant)
            builder.Entity<HC>()
                .HasOne(h => h.Company)
                .WithMany() // no navigation collection required, or you may add one on Company if desired
                .HasForeignKey(h => h.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Payorder>()
            .HasOne(p => p.BL)
            .WithMany(b => b.Payorders)
            .HasForeignKey(p => p.BLId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payorder>()
                .HasOne(p => p.LC)
                .WithMany(lc => lc.Payorders)
                .HasForeignKey(p => p.LCId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payorder>()
                .HasOne(p => p.Company)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            // PayorderHeader -> Company
            builder.Entity<PayorderHeader>()
                .HasOne(ph => ph.Company)
                .WithMany() // or .WithMany(c => c.PayorderHeaders) if you have collection on Company
                .HasForeignKey(ph => ph.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);


            // Consignment -> LC (One-to-One: One Consignment has One LC)
            builder.Entity<Consignment>()
                .HasOne(c => c.LC)
                .WithOne() // If LC has ICollection<Consignment>, use: .WithMany(lc => lc.Consignments)
                .HasForeignKey<Consignment> (c => c.LCId)
                .OnDelete(DeleteBehavior.Restrict);

            // Consignment -> Company (Many-to-One: Many Consignments belong to one Company)
            builder.Entity<Consignment>()
                .HasOne(c => c.Company)
                .WithMany(c => c.Consignments) // Company has many Consignments
                .HasForeignKey(c => c.CompanyId)
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

            builder.Entity<City>()
                .HasQueryFilter(c => !EnableTenantFilter || !_currentCompanyId.HasValue || c.CompanyId == _currentCompanyId);

            builder.Entity<Port>()
                .HasQueryFilter(p => !EnableTenantFilter || !_currentCompanyId.HasValue || p.CompanyId == _currentCompanyId);




            builder.Entity<LC>()
                .HasQueryFilter(lc => !EnableTenantFilter || !_currentCompanyId.HasValue || lc.CompanyId == _currentCompanyId);

            builder.Entity<BL>()
                .HasQueryFilter(bl => !EnableTenantFilter || !_currentCompanyId.HasValue || bl.CompanyId == _currentCompanyId);

            builder.Entity<PendingBL>()
                .HasQueryFilter(pb => !EnableTenantFilter || !_currentCompanyId.HasValue || pb.CompanyId == _currentCompanyId);

            builder.Entity<BLItem>()
                .HasQueryFilter(bi => !EnableTenantFilter || !_currentCompanyId.HasValue || bi.CompanyId == _currentCompanyId);

            builder.Entity<IGM>()
                .HasQueryFilter(igm => !EnableTenantFilter || !_currentCompanyId.HasValue || igm.CompanyId == _currentCompanyId);

            builder.Entity<DutyCharge>()
                .HasQueryFilter(dc => !EnableTenantFilter || !_currentCompanyId.HasValue || dc.CompanyId == _currentCompanyId);
            
            builder.Entity<Payorder>()
                .HasQueryFilter(p => !EnableTenantFilter || !_currentCompanyId.HasValue || p.CompanyId == _currentCompanyId);

            builder.Entity<PayorderHeader>()
                .HasQueryFilter(p => !EnableTenantFilter || !_currentCompanyId.HasValue || p.CompanyId == _currentCompanyId);

            // Global Query Filter for Consignment
            builder.Entity<Consignment>()
                .HasQueryFilter(c => !EnableTenantFilter || !_currentCompanyId.HasValue || c.CompanyId == _currentCompanyId);

        }

        // Optional: expose a method to set current tenant at runtime
        public void SetTenant(int? companyId)
        {
            _currentCompanyId = companyId;
        }
    }
}
