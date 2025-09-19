using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            //await LoadCompanyInfoAsync();
            //_context.EnableTenantFilter = false;
            var users = await _context.Employees.Include(u => u.ApplicationUser).ToListAsync();
            //_context.EnableTenantFilter = true;
            Console.WriteLine($"Found {users.Count} employees");
            return View(users);
        }

        // Create User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                // Set default values
                //user.UserName = user.Email;
                //user.NormalizedUserName = user.Email.ToUpper();
                //user.SecurityStamp = Guid.NewGuid().ToString();
                //user.CreatedAt = DateTime.UtcNow;
                //_context.Add(user);
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
    }
}
