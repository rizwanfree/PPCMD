using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<ApplicationUser> _userManager;

        public BaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            // Set current tenant for EF Core global filter
            var userId = HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                _context.SetTenant(user?.CompanyId);
            }
        }

        protected async Task LoadCompanyInfoAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.userFullName = $"{user.FirstName} {user.LastName}";
            }
            if (user?.CompanyId != null)
            {
                var company = await _context.Companies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == user.CompanyId);

                if (company != null)
                {
                    ViewBag.CompanyName = company.CompanyName;
                    ViewBag.CompanyLogo = company.Website ?? "/images/logo-placeholder.png";
                    //ViewBag.userFullName = user.FullName ?? "Hello User";
                    ViewBag.userProfilePicture = user.ProfilePictureUrl ?? "/images/user.jpg";
                    // you can store logo URL in DB later, for now Website is optional
                }
            }
        }
    }
}
