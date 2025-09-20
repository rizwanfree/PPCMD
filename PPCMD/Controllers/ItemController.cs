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


        // ----------------------------
        // DUTY TYPES MANAGEMENT
        // ----------------------------

        // GET: DutyTypes
        public async Task<IActionResult> DutyTypes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var dutyTypes = await _context.DutyTypes
                .Where(d => d.CompanyId == user.CompanyId.Value)
                .AsNoTracking()
                .ToListAsync();

            return View(dutyTypes);
        }


        // POST: DutyTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDutyType(DutyType dutyType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"❌ Model binding failed for {kvp.Key}: {error.ErrorMessage}");
                    }
                }
                TempData["ErrorMessage"] = "Invalid duty type data.";
                return RedirectToAction(nameof(DutyTypes));
            }

            // ✅ should run now if binding works
            dutyType.CompanyId = user.CompanyId.Value;

            try
            {
                _context.DutyTypes.Add(dutyType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Duty type created successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Duty Not Added!!!");
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Error saving duty type.";
            }

            return RedirectToAction(nameof(DutyTypes));
        }

        // GET: DutyTypes/Edit/5
        public async Task<IActionResult> EditDutyType(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var dutyType = await _context.DutyTypes
                .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == user.CompanyId.Value);

            if (dutyType == null) return NotFound();

            return View(dutyType);
        }

        // POST: DutyTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDutyType(int id, DutyType dutyType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            if (id != dutyType.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.DutyTypes
                    .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == user.CompanyId.Value);

                if (existing == null) return NotFound();

                existing.Name = dutyType.Name;
                existing.Description = dutyType.Description;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(DutyTypes));
            }
            return View(dutyType);
        }

        // POST: DutyTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDutyType(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var dutyType = await _context.DutyTypes
                .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == user.CompanyId.Value);

            if (dutyType == null) return NotFound();

            _context.DutyTypes.Remove(dutyType);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DutyTypes));
        }
    }
}

