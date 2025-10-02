using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class PayorderController : BaseController
    {
        public PayorderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager)
        {
        }

        // GET: List of Payorder Headers
        public async Task<IActionResult> PayorderHeaders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var headers = await _context.PayorderHeaders
                .Where(h => h.CompanyId == user.CompanyId.Value)
                .AsNoTracking()
                .ToListAsync();

            return View(headers);
        }

        // POST: Create Header
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHeader(PayorderHeader header)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid header data.";
                return RedirectToAction(nameof(PayorderHeaders));
            }

            header.CompanyId = user.CompanyId.Value;
            _context.PayorderHeaders.Add(header);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Header created successfully.";
            return RedirectToAction(nameof(PayorderHeaders));
        }

        // POST: Edit Header
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHeader(int id, PayorderHeader header)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            if (id != header.Id) return NotFound();

            var existing = await _context.PayorderHeaders
                .FirstOrDefaultAsync(h => h.Id == id && h.CompanyId == user.CompanyId.Value);

            if (existing == null) return NotFound();

            // ✅ Fix: Update Name and Description
            existing.Name = header.Name;
            existing.Description = header.Description;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Header updated successfully.";
            return RedirectToAction(nameof(PayorderHeaders));
        }

        // POST: Delete Header
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHeader(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var header = await _context.PayorderHeaders
                .FirstOrDefaultAsync(h => h.Id == id && h.CompanyId == user.CompanyId.Value);

            if (header == null) return NotFound();

            _context.PayorderHeaders.Remove(header);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Header deleted successfully.";
            return RedirectToAction(nameof(PayorderHeaders));
        }
    }
}
