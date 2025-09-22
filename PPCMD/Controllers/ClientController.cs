using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class ClientController : BaseController
    {
        // Constructor: injects ApplicationDbContext and UserManager for multi-tenant & user operations
        public ClientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
             : base(context, userManager)
        {
        }

        // GET: Clients
        // Returns a list of all clients for the current company, including related ClientType and Emails.
        // Note: In production, you may want to filter out soft-deleted clients using c.IsDeleted == false
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients
                .Include(c => c.ClientType) // Include client type for display
                .Include(c => c.Emails)     // Include emails to show first email if needed
                .ToListAsync();

            return View(clients);
        }

        // GET: Clients/Details/5
        // Returns detailed information about a single client by ID.
        // Includes related data like ClientType and Emails.
        // Returns NotFound() if client does not exist.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.ClientType)
                .Include(c => c.Emails)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // GET: Clients/Create
        // Returns the Create Client form.
        // Populates ViewBag.ClientTypes for the dropdown.
        public IActionResult Create()
        {
            var clientTypes = _context.ClientTypes.ToList();

            // Convert to SelectListItem for dropdown
            ViewBag.ClientTypes = clientTypes.Select(ct => new SelectListItem
            {
                Value = ct.Id.ToString(),
                Text = ct.Name
            });

            return View();
        }

        // POST: Clients/Create
        // Receives form data for creating a new client.
        // Accepts a list of Emails from the form.
        // Assigns CompanyId from logged-in user (multi-tenant) and sets CreatedAt.
        // Adds client to database and saves changes.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client, List<string> Emails)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                // Multi-tenant: assign client to the user's company
                client.CompanyId = user!.CompanyId ?? 0;
                client.CreatedAt = DateTime.UtcNow;

                // Add emails from form input
                foreach (var email in Emails.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    client.Emails.Add(new ClientEmail { Email = email });
                }

                _context.Add(client);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If model validation fails, re-populate ClientTypes for dropdown
            ViewData["ClientTypes"] = _context.Set<ClientType>().ToList();
            return View(client);
        }

        // GET: Clients/Edit/5
        // Returns the Edit Client form populated with existing client data.
        // Includes emails collection for dynamic editing.
        // Populates ClientTypes dropdown using SelectList for proper pre-selection.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.Emails)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return NotFound();

            // Populate dropdown
            ViewBag.ClientTypes = new SelectList(
                _context.ClientTypes.ToList(),
                "Id",          // value field
                "Name",        // text field
                client.ClientTypeId // selected value
            );

            return View(client);
        }

        // POST: Clients/Edit/5
        // Receives updated client data from form.
        // Updates scalar fields (name, contact, address, etc.).
        // Syncs emails: adds new, updates existing, removes deleted.
        // Preserves CreatedAt, sets UpdatedAt for audit.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Clients
                    .Include(c => c.Emails)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existing == null) return NotFound();

                // Populate dropdown again in case of validation error
                ViewBag.ClientTypes = new SelectList(
                    _context.ClientTypes.ToList(),
                    "Id",
                    "Name",
                    client.ClientTypeId
                );

                // Update scalar fields
                existing.ClientName = client.ClientName;
                existing.ContactPerson = client.ContactPerson;
                existing.Phone = client.Phone;
                existing.Mobile = client.Mobile;
                existing.Address = client.Address;
                existing.GST = client.GST;
                existing.NTN = client.NTN;
                existing.DirectorName = client.DirectorName;
                existing.NIC = client.NIC;
                existing.DirectorAddress = client.DirectorAddress;
                existing.ClientTypeId = client.ClientTypeId;
                existing.UpdatedAt = DateTime.UtcNow;

                // 🔹 Sync Emails
                var postedEmails = client.Emails ?? new List<ClientEmail>();

                // 1️⃣ Remove emails that are no longer in posted form
                foreach (var oldEmail in existing.Emails.ToList())
                {
                    if (!postedEmails.Any(e => e.Id == oldEmail.Id))
                        _context.ClientEmails.Remove(oldEmail);
                }

                // 2️⃣ Update existing emails or add new ones
                foreach (var postedEmail in postedEmails)
                {
                    if (postedEmail.Id == 0)
                    {
                        // New email
                        existing.Emails.Add(new ClientEmail
                        {
                            Email = postedEmail.Email,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        // Update existing email
                        var dbEmail = existing.Emails.FirstOrDefault(e => e.Id == postedEmail.Id);
                        if (dbEmail != null)
                        {
                            dbEmail.Email = postedEmail.Email;
                            dbEmail.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientTypes"] = _context.ClientTypes.ToList();
            return View(client);
        }

        // GET: Clients/Delete/5
        // Returns a confirmation page for soft-deleting a client.
        // Does not physically remove the client to preserve historical data.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.ClientType)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        // Soft deletes the client by setting IsDeleted = true.
        // Updates UpdatedAt timestamp for audit.
        // Preserves all historical relationships (jobs, finances, emails).
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            client.IsDeleted = true;
            client.UpdatedAt = DateTime.UtcNow;

            _context.Update(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method: checks if a client exists by ID
        // Useful for concurrency checks and validation
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
