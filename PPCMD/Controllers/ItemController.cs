using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {

        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    : base(context, userManager)
        {
        }


        // GET: Items
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items
                .Include(i => i.Duties)
                .ThenInclude(d => d.DutyType)
                .AsNoTracking()
                .ToListAsync();

            return View(items);
        }

        // GET: Items/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.DutyTypes = await _context.DutyTypes.AsNoTracking().ToListAsync();
            return View(new Item());
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                // Attach companyId
                var user = await _userManager.GetUserAsync(User);
                if (user?.CompanyId != null)
                {
                    item.CompanyId = user.CompanyId.Value;
                }

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new {id = item.ItemID});
            }
            ViewBag.DutyTypes = await _context.DutyTypes.AsNoTracking().ToListAsync();
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Items
                .Include(i => i.Duties)
                .ThenInclude(d => d.DutyType)
                .FirstOrDefaultAsync(i => i.ItemID == id);

            if (item == null)
                return NotFound();

            ViewBag.DutyTypes = await _context.DutyTypes.AsNoTracking().ToListAsync();
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item updatedItem)
        {
            if (id != updatedItem.ItemID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(updatedItem);

            var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.ItemID == id);

            if (existingItem == null)
                return NotFound();

            // ✅ Only update allowed fields
            existingItem.ItemName = updatedItem.ItemName;
            existingItem.HSCode = updatedItem.HSCode;
            existingItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Item not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Item deleted successfully.";
            return RedirectToAction(nameof(Index));
        }



        // POST: Items/AddDuty
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDuty(int itemId, int dutyTypeId, decimal rate, bool isPercentage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var duty = new ItemDuty
            {
                ItemID = itemId,
                DutyTypeId = dutyTypeId,
                Rate = rate,
                IsPercentage = isPercentage,
                CompanyId = user.CompanyId.Value
            };

            _context.ItemDuties.Add(duty);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = itemId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDuty(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var duty = await _context.ItemDuties
                .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == user.CompanyId.Value);

            if (duty == null)
            {
                return NotFound();
            }

            _context.ItemDuties.Remove(duty);
            await _context.SaveChangesAsync();

            // Redirect back to the edit page for the same item
            return RedirectToAction(nameof(Edit), new { id = duty.ItemID });
        }
    }
}

