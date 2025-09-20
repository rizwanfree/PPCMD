using Microsoft.AspNetCore.Authorization;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        public UserController(
                            ApplicationDbContext context, 
                            UserManager<ApplicationUser> userManager, 
                            IWebHostEnvironment env, 
                            RoleManager<IdentityRole> roleManager)
            : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _roleManager = roleManager;
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

        // GET: /User/CreateEmployee
        [HttpGet]
        [Authorize(Roles = "Admin")] // only Admins create employees (change if needed)
        public async Task<IActionResult> CreateEmployee()
        {
            await LoadCompanyInfoAsync();
            var emp = new Employee();
            return View("EmployeeForm", emp);
        }

        // POST: /User/CreateEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee(Employee emp)
        {
            await LoadCompanyInfoAsync();

            if (!ModelState.IsValid)
                return View("EmployeeForm", emp);

            // Determine current company from logged in user
            var currentUser = await _userManager.GetUserAsync(User);
            var companyId = currentUser?.CompanyId;
            if (companyId == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot determine your company. Contact admin.");
                return View("EmployeeForm", emp);
            }

            // assign tenant
            emp.CompanyId = companyId.Value;
            emp.CreatedAt = DateTime.UtcNow;

            // Handle profile picture
            if (emp.ImageFile != null)
            {
                var picResult = await SaveEmployeeImageAsync(emp.ImageFile, companyId.Value);
                if (!picResult.success)
                {
                    ModelState.AddModelError(string.Empty, picResult.errorMessage!);
                    return View("EmployeeForm", emp);
                }

                emp.ProfilePictureUrl = picResult.relativePath;
            }

            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // your index listing
        }


        // GET: /User/EditEmployee/5
        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            await LoadCompanyInfoAsync();

            var emp = await _context.Employees
                .Include(e => e.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emp == null) return NotFound();

            // Authorization: admin can edit anyone; non-admin only their own account
            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") &&
                emp.ApplicationUser?.Id != currentUserId)
            {
                return Forbid();
            }

            return View("EmployeeForm", emp);
        }


        // POST: /User/EditEmployee/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(int id, Employee model)
        {
            await LoadCompanyInfoAsync();

            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("EmployeeForm", model);

            var emp = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emp == null) return NotFound();

            // Authorization check
            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") &&
                emp.ApplicationUserId != currentUserId)
            {
                return Forbid();
            }

            // Update allowed fields only (do not allow changing CompanyId or ApplicationUserId here)
            emp.FirstName = model.FirstName;
            emp.LastName = model.LastName;
            emp.Designation = model.Designation;
            emp.Email = model.Email;
            emp.Mobile = model.Mobile;
            emp.Address = model.Address;
            emp.CNIC = model.CNIC;
            emp.IsActive = model.IsActive;
            emp.UpdatedAt = DateTime.UtcNow;

            // If new image uploaded, replace
            if (model.ImageFile != null)
            {
                // optional: delete old file
                if (!string.IsNullOrEmpty(emp.ProfilePictureUrl))
                {
                    try
                    {
                        var existingPath = Path.Combine(_env.WebRootPath, emp.ProfilePictureUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(existingPath))
                            System.IO.File.Delete(existingPath);
                    }
                    catch { /* swallow - not critical */ }
                }

                var picResult = await SaveEmployeeImageAsync(model.ImageFile, emp.CompanyId ?? 0);
                if (!picResult.success)
                {
                    ModelState.AddModelError(string.Empty, picResult.errorMessage!);
                    return View("EmployeeForm", model);
                }
                emp.ProfilePictureUrl = picResult.relativePath;
            }

            _context.Employees.Update(emp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // Helper: file saving
        private async Task<(bool success, string? relativePath, string? errorMessage)> SaveEmployeeImageAsync(IFormFile file, int companyId)
        {
            // validate
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                return (false, null, "Only JPG/PNG/WEBP images are allowed.");

            if (file.Length > 2 * 1024 * 1024)
                return (false, null, "Image must be smaller than 2 MB.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "employees", companyId.ToString());
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsFolder, fileName);
            using var stream = System.IO.File.Create(fullPath);
            await file.CopyToAsync(stream);

            // return web-relative path
            var relative = $"/uploads/employees/{companyId}/{fileName}";
            return (true, relative, null);
        }




        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GrantAccess(int employeeId)
        {
            var emp = await _context.Employees.Include(e => e.Company).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (emp == null) return NotFound();

            if (!string.IsNullOrEmpty(emp.ApplicationUserId))
            {
                TempData["ErrorMessage"] = "This employee already has a user account.";
                return RedirectToAction(nameof(Index));
            }

            // Pre-fill with suggested username (e.g. emp-firstname.lastname-company)
            var suggestedUsername = $"{emp.FirstName.ToLower()}.{emp.LastName.ToLower()}";

            var user = new ApplicationUser
            {
                UserName = $"{suggestedUsername}-{emp.Company.License}",
                Email = emp.Email,
                EmployeeId = emp.Id,
                CompanyId = emp.CompanyId
            };

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GrantAccess(ApplicationUser user, string password, string? role)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
                ModelState.AddModelError("UserName", "Username is required.");

            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("", "Password is required.");

            if (!ModelState.IsValid)
                return View(user);

            // Check if username is unique
            var existingUser = await _userManager.FindByNameAsync(user.UserName!);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "This username is already taken.");
                return View(user);
            }

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(user);
            }

            if (!string.IsNullOrEmpty(role))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);
            }

            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.Id == user.EmployeeId);
            if (emp != null)
            {
                emp.ApplicationUserId = user.Id;
                _context.Update(emp);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Access granted to {user.UserName}";
            return RedirectToAction(nameof(Index));
        }



        // GET: /User/Profile?employeeId=123   (employeeId optional; admin can pass employeeId)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile(int? employeeId = null)
        {
            //await LoadCompanyInfoAsync();

            Employee emp;
            if (employeeId.HasValue && User.IsInRole("Admin"))
            {
                emp = await _context.Employees
                    .Include(e => e.ApplicationUser)
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(e => e.Id == employeeId.Value);
            }
            else
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return Challenge();
                emp = await _context.Employees
                    .Include(e => e.ApplicationUser)
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(e => e.Id == currentUser.EmployeeId);
            }

            if (emp == null) return NotFound();

            return View("EmployeeProfile", emp);
        }

        // POST: /User/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(Employee model, string? currentPassword, string? newPassword, string? confirmPassword)
        {
            await LoadCompanyInfoAsync();

            // find existing employee
            var emp = await _context.Employees
                        .Include(e => e.ApplicationUser)
                        .FirstOrDefaultAsync(e => e.Id == model.Id);

            if (emp == null) return NotFound();

            // Authorization:
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;
            var editingOther = !(emp.ApplicationUserId == currentUserId);

            if (!User.IsInRole("Admin") && editingOther)
            {
                // non-admin cannot edit other people's profiles
                return Forbid();
            }

            // validate confirm-password if provided
            if (!string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(confirmPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "New password and confirmation do not match.");
                    return View("EmployeeProfile", model);
                }
            }

            // Update allowed employee fields (don't change CompanyId/ApplicationUserId here)
            emp.FirstName = model.FirstName;
            emp.LastName = model.LastName;
            emp.Designation = model.Designation;
            emp.Email = model.Email;
            emp.Mobile = model.Mobile;
            emp.Address = model.Address;
            emp.CNIC = model.CNIC;
            emp.UpdatedAt = DateTime.UtcNow;

            // handle image upload
            if (model.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(emp.ProfilePictureUrl))
                {
                    try
                    {
                        var existingPath = Path.Combine(_env.WebRootPath, emp.ProfilePictureUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(existingPath))
                            System.IO.File.Delete(existingPath);
                    }
                    catch { }
                }

                var picResult = await SaveEmployeeImageAsync(model.ImageFile, emp.CompanyId ?? 0);
                if (!picResult.success)
                {
                    ModelState.AddModelError(string.Empty, picResult.errorMessage!);
                    return View("EmployeeProfile", emp);
                }
                emp.ProfilePictureUrl = picResult.relativePath;
            }

            _context.Employees.Update(emp);
            await _context.SaveChangesAsync();

            // Password handling:
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (emp.ApplicationUser == null)
                {
                    // can't change password for a non-user account
                    ModelState.AddModelError("", "Employee does not have a system account. Grant access first.");
                    return View("EmployeeProfile", emp);
                }

                if (User.IsInRole("Admin") && editingOther)
                {
                    // Admin resetting someone else's password -> use reset token
                    var targetUser = await _userManager.FindByIdAsync(emp.ApplicationUserId);
                    if (targetUser == null)
                    {
                        ModelState.AddModelError("", "Associated user account not found.");
                        return View("EmployeeProfile", emp);
                    }

                    var token = await _userManager.GeneratePasswordResetTokenAsync(targetUser);
                    var resetResult = await _userManager.ResetPasswordAsync(targetUser, token, newPassword);
                    if (!resetResult.Succeeded)
                    {
                        foreach (var err in resetResult.Errors)
                            ModelState.AddModelError("", err.Description);
                        return View("EmployeeProfile", emp);
                    }
                }
                else
                {
                    // Non-admin changing own password - require currentPassword
                    if (currentUser == null)
                    {
                        return Challenge();
                    }

                    var changeResult = await _userManager.ChangePasswordAsync(currentUser, currentPassword ?? "", newPassword);
                    if (!changeResult.Succeeded)
                    {
                        foreach (var err in changeResult.Errors)
                            ModelState.AddModelError("", err.Description);
                        return View("EmployeeProfile", emp);
                    }
                }
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Profile", new { employeeId = emp.Id });
        }
    }
}
