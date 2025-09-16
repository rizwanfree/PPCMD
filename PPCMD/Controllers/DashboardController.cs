using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PPCMD.Data;
using PPCMD.Models;
using PPCMD.Repositories;

namespace PPCMD.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        private readonly Repository<City> _city;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
            : base(context, userManager)
        {
            _env = env;
            _city = new Repository<City>(context);
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

        // GET: /Dashboard/Cities
        [HttpGet]
        public async Task<IActionResult> Cities()
        {
            await LoadCompanyInfoAsync();
            var cities = await _city.GetAllAsync();
            return View(cities);
        }

        // POST: /Dashboard/CreateCity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCity(City model)
        {
            await LoadCompanyInfoAsync();
            if (!ModelState.IsValid)
                return RedirectToAction("Cities");

            await _city.AddAsync(model);
            TempData["SuccessMessage"] = "City added successfully!";
            return RedirectToAction(nameof(Cities));
        }

        // POST: /Dashboard/EditCity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(City model)
        {
            await LoadCompanyInfoAsync();

            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Cities));

            var city = await _city.GetByIdAsync(model.Id, new QueryOption<City> { });
            if (city == null)
                return NotFound();

            city.Name = model.Name;
            await _city.UpdateAsync(city);
            

            TempData["SuccessMessage"] = "City updated successfully!";
            return RedirectToAction(nameof(Cities));
        }
    }
}
