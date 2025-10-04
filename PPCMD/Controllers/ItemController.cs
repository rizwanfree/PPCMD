using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    [Authorize] // ✅ Ensure only authenticated users can access this controller
    public class ItemController : BaseController
    {
        // 🔧 Constructor: Injects DbContext and UserManager into BaseController
        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager)
        {
        }

        // ================================================
        // GET: Items
        // Returns a list of all items with their duties.
        // ================================================
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items
                .Include(i => i.Duties)          // Load duties for each item
                .ThenInclude(d => d.DutyType)    // Also load the duty type details
                .AsNoTracking()                  // No tracking needed for read-only view
                .ToListAsync();

            return View(items); // Pass list to view
        }

        // ================================================
        // GET: Items/Create
        // Shows empty form for creating a new item.
        // ================================================
        public async Task<IActionResult> Create()
        {
            // Populate dropdown list with all duty types for selection
            ViewBag.DutyTypes = await _context.DutyTypes.AsNoTracking().ToListAsync();
            return View(new Item()); // Return empty item model
        }

        // ================================================
        // POST: Items/Create
        // Saves new item into DB, attaches companyId.
        // ================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {
            if (ModelState.IsValid) // ✅ Only proceed if model is valid
            {
                // Attach current company ID from logged-in user
                var user = await _userManager.GetUserAsync(User);
                if (user?.CompanyId != null)
                {
                    item.CompanyId = user.CompanyId.Value;
                }

                _context.Add(item);             // Add to DbSet
                await _context.SaveChangesAsync(); // Save changes
                return RedirectToAction(nameof(Edit), new { id = item.Id }); // Redirect to Edit page
            }

            // If invalid, re-populate duty types and return view with errors
            ViewBag.DutyTypes = await _context.DutyTypes.AsNoTracking().ToListAsync();
            return View(item);
        }

        // ================================================
        // GET: Items/Edit/{id}
        // Loads an existing item with its duties for editing.
        // ================================================
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Items
                .Include(i => i.Duties)          // Include duties
                    .ThenInclude(d => d.DutyType) // Include duty type details
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item != null)
            {
                // ✅ Sort duties by order before displaying
                item.Duties = item.Duties.OrderBy(d => d.Order).ToList();
            }

            if (item == null)
                return NotFound(); // Return 404 if item not found

            ViewBag.DutyTypes = await _context.DutyTypes.AsNoTracking().ToListAsync();
            return View(item);
        }

        // ================================================
        // POST: Items/Edit/{id}
        // Updates item details (name, HSCode, etc.)
        // ================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item updatedItem)
        {
            if (id != updatedItem.Id)
                return NotFound(); // Ensure route id matches model id

            if (!ModelState.IsValid)
                return View(updatedItem); // Return with validation errors

            var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);

            if (existingItem == null)
                return NotFound(); // Item not found

            // ✅ Update only allowed fields
            existingItem.ItemName = updatedItem.ItemName;
            existingItem.HSCode = updatedItem.HSCode;
            existingItem.UpdatedAt = DateTime.UtcNow; // Track last update time

            await _context.SaveChangesAsync(); // Commit changes

            return RedirectToAction(nameof(Index)); // Redirect to item list
        }

        // ================================================
        // POST: Items/Delete/{id}
        // Deletes an item (and cascades duties if configured).
        // ================================================
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

            _context.Items.Remove(item);        // Remove item
            await _context.SaveChangesAsync();  // Save changes

            TempData["SuccessMessage"] = "Item deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ================================================
        // POST: Items/AddDuty
        // Adds a new duty to an item, appending at the end.
        // ================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDuty(int itemId, int dutyTypeId, decimal rate, bool isPercentage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            // Find current maximum order for this item's duties
            var maxOrder = await _context.ItemDuties
                .Where(d => d.ItemID == itemId && d.CompanyId == user.CompanyId)
                .Select(d => (int?)d.Order)
                .MaxAsync() ?? -1;

            // Create new duty with next order value
            var duty = new ItemDuty
            {
                ItemID = itemId,
                DutyTypeId = dutyTypeId,
                Rate = rate,
                IsPercentage = isPercentage,
                CompanyId = user.CompanyId.Value,
                Order = maxOrder + 1 // 👈 Always append at end
            };

            _context.ItemDuties.Add(duty);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = itemId });
        }

        // ================================================
        // POST: Items/DeleteDuty/{id}
        // Deletes a duty from an item and normalizes order.
        // ================================================
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

            var itemId = duty.ItemID;

            // 1️⃣ Remove selected duty
            _context.ItemDuties.Remove(duty);
            await _context.SaveChangesAsync();

            // 2️⃣ Normalize remaining duty orders
            var duties = await _context.ItemDuties
                .Where(d => d.ItemID == itemId && d.CompanyId == user.CompanyId)
                .OrderBy(d => d.Order)
                .ToListAsync();

            for (int i = 0; i < duties.Count; i++)
            {
                duties[i].Order = i; // Reset order sequentially
            }

            await _context.SaveChangesAsync();

            // 3️⃣ Redirect back to item edit
            return RedirectToAction(nameof(Edit), new { id = itemId });
        }

        

        // ================================================
        // POST: Items/ReorderDuties
        // Accepts JSON of reordered duties and updates DB.
        // ================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderDuties([FromBody] List<DutyOrderDto> orderList)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            // Extract duty IDs from request
            var dutyIds = orderList.Select(o => o.Id).ToList();

            // Load duties that belong to this company and match IDs
            var duties = await _context.ItemDuties
                .Where(d => dutyIds.Contains(d.Id) && d.CompanyId == user.CompanyId)
                .ToListAsync();

            // Update order values
            foreach (var o in orderList)
            {
                var duty = duties.FirstOrDefault(d => d.Id == o.Id);
                if (duty != null) duty.Order = o.Order;
            }

            await _context.SaveChangesAsync();

            // Return duties back sorted by order
            return Json(duties.OrderBy(d => d.Order));
        }

        // Helper DTO for reordering
        public class DutyOrderDto
        {
            public int Id { get; set; }     // Duty ID
            public int Order { get; set; }  // New order value
        }
    }
}
