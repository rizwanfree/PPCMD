using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;
using System.Threading.Tasks;

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
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {           

            // HttpContext is available here
            var userId = HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Make sure we don't trigger tracking
                var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
                if (user?.CompanyId != null)
                {
                    _context.SetTenant(user.CompanyId);
                }
            }
            LoadCompanyInfoAsync().GetAwaiter().GetResult();
            base.OnActionExecuting(context);
        }

        protected async Task LoadCompanyInfoAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.userFullName = $"{user.Employee!.FirstName} {user.Employee.LastName}";
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
                    ViewBag.userProfilePicture = user.Employee?.ProfilePictureUrl ?? "/images/user.jpg";
                }
            }
        }
    }
}
