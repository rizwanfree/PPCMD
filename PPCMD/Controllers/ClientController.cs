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
        public ClientController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
             : base(context, userManager)
        {
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients
                .Include(c => c.ClientType)
                .Include(c => c.Emails)
                .ToListAsync();

            return View(clients);
        }

        // GET: Clients/Details/5
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
        public IActionResult Create()
        {
            var clientTypes = _context.ClientTypes.ToList();

            ViewBag.ClientTypes = clientTypes.Select(ct => new SelectListItem
            {
                Value = ct.Id.ToString(),
                Text = ct.Name
            });

            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client, List<string> Emails)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                client.CompanyId = user!.CompanyId ?? 0;
                client.CreatedAt = DateTime.UtcNow;

                // Add emails from form
                foreach (var email in Emails.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    client.Emails.Add(new ClientEmail { Email = email });
                }

                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientTypes"] = _context.Set<ClientType>().ToList();
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.Emails)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return NotFound();

            ViewData["ClientTypes"] = _context.Set<ClientType>().ToList();
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existing == null) return NotFound();

                    client.CompanyId = existing.CompanyId; // keep tenant
                    client.CreatedAt = existing.CreatedAt; // preserve created date
                    client.UpdatedAt = DateTime.UtcNow;

                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientTypes"] = _context.Set<ClientType>().ToList();
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.ClientType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
