// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPCMD.Data;
using PPCMD.Models;
using System.ComponentModel.DataAnnotations;

namespace PPCMD.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger
            //IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            //_emailSender = emailSender;

            _context = context;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; } = string.Empty;

            [Required]
            [Display(Name = "License")]
            public int License { get; set; }

            [Required]
            [Display(Name = "NTN")]
            public string NTN { get; set; } = string.Empty;

            [Required]
            [Display(Name = "STN")]
            public string STN { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Proprietor / CEO Name")]
            public string ProprietorName { get; set; } = string.Empty;

            [Required]
            [Display(Name = "CNIC")]
            public string CNIC { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Company Email")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Website")]
            public string? Website { get; set; }

            [Required]
            [Display(Name = "Landline 1")]
            public string Landline1 { get; set; } = string.Empty;

            [Display(Name = "Landline 2")]
            public string? Landline2 { get; set; }

            [Required]
            [Display(Name = "Mobile")]
            public string Mobile { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                

                // 1️⃣ Create Company
                var company = new Company
                {
                    CompanyName = Input.CompanyName,
                    License = Input.License,
                    NTN = Input.NTN,
                    STN = Input.STN,
                    ProprietorName = Input.ProprietorName,
                    CNIC = Input.CNIC,
                    Email = Input.Email,
                    Website = Input.Website,
                    Landline1 = Input.Landline1,
                    Landline2 = Input.Landline2,
                    Mobile = Input.Mobile,
                    Address = Input.Address
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // 2️⃣ Create Employee record for Admin
                var adminEmployee = new Employee
                {
                    FirstName = string.Empty, // or take from registration form
                    LastName = string.Empty,          // can leave empty or split ProprietorName
                    Email = string.Empty,
                    CNIC = string.Empty,
                    Designation = "Administrator",
                    IsActive = true,
                    CompanyId = company.Id
                };

                _context.Employees.Add(adminEmployee);
                await _context.SaveChangesAsync(); // Need employee.Id for linking

                // Create ApplicationUser and Link to Employee

                ApplicationUser user = new ApplicationUser
                {
                    UserName = $"admin-{company.License}",
                    CompanyId = company.Id,
                    EmployeeId = adminEmployee.Id
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    adminEmployee.ApplicationUserId = user.Id;
                    _context.Employees.Update(adminEmployee);
                    await _context.SaveChangesAsync();

                    // 3️⃣ Ensure Admin role exists
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    // 4️⃣ Assign Admin role
                    await _userManager.AddToRoleAsync(user, "Admin");

                    _logger.LogInformation("Admin user created for company {CompanyName}", company.CompanyName);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
