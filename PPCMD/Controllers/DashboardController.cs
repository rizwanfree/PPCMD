using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
            : base(context, userManager)
        {
            _env = env;
        }

        // Dashboard Home
        public async Task<IActionResult> Index()
        {
            await LoadCompanyInfoAsync();
            return View();
        }

        // GET: /Dashboard/UserProfile
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            await LoadCompanyInfoAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /Dashboard/UserProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(ApplicationUser model)
        {
            await LoadCompanyInfoAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return View(user);

            // Update profile fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.CNIC = model.CNIC;
            user.Mobile = model.Mobile;
            user.Address = model.Address;

            // Handle profile picture
            if (model.ImageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var filePath = Path.Combine(_env.WebRootPath, "images/users", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                user.ProfilePictureUrl = $"/images/users/{fileName}";
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(UserProfile));
        }
    }
}
