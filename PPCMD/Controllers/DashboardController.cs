using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index()
        {
            //await LoadCompanyInfoAsync();
            return View();
        }

        // GET: /Dashboard/UserProfile
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            //await LoadCompanyInfoAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /Dashboard/UserProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfile(ApplicationUser model)
        {
            //await LoadCompanyInfoAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
                return View(user);

            // Update profile fields
            //user.FirstName = model.FirstName;
            //user.LastName = model.LastName;
            //user.CNIC = model.CNIC;
            //user.Mobile = model.Mobile;
            //user.Address = model.Address;

            //// Handle profile picture
            //if (model.ImageFile != null)
            //{
            //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
            //    var filePath = Path.Combine(_env.WebRootPath, "images/users", fileName);

            //    using (var stream = System.IO.File.Create(filePath))
            //    {
            //        await model.ImageFile.CopyToAsync(stream);
            //    }

            //    user.ProfilePictureUrl = $"/images/users/{fileName}";
            //}

            //user.UpdatedAt = DateTime.UtcNow;

            //await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(UserProfile));
        }


        // GET: /Dashboard/CompanyProfile
        [HttpGet]
        public async Task<IActionResult> CompanyProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == null)
                return NotFound();

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == user.CompanyId);

            if (company == null)
                return NotFound();

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompanyProfile(Company model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (company == null)
                return NotFound();

            // ✅ Update fields safely
            company.NTN = model.NTN;
            company.STN = model.STN;
            company.ProprietorName = model.ProprietorName;
            company.CNIC = model.CNIC;
            company.Email = model.Email;
            company.Website = model.Website;
            company.Landline1 = model.Landline1;
            company.Landline2 = model.Landline2;
            company.Mobile = model.Mobile;
            company.Address = model.Address;
            //company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Company profile updated successfully!";
            return RedirectToAction(nameof(CompanyProfile));
        }


        // GET: /Dashboard/Cities
        [HttpGet]
        public async Task<IActionResult> Cities()
        {
            //await LoadCompanyInfoAsync();
            var cities = await _city.GetAllAsync();
            return View(cities);
        }

        // POST: /Dashboard/CreateCity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCity(City model)
        {
            //await LoadCompanyInfoAsync();
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
           // await LoadCompanyInfoAsync();

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
