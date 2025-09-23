using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class MaritimeController : BaseController
    {
        public MaritimeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(context, userManager)
        {
        }


        // Generic Index: pass type = "Terminal", "Lolo", "ShippingLine"
        public IActionResult Index()
        {
            // Fetch each type separately
            ViewBag.Terminals = _context.Terminals.ToList();

            ViewBag.Lolos = _context.Lolos.ToList();

            ViewBag.ShippingLines = _context.ShippingLines.ToList();

            return View();
        }



        // GET: /Maritime/CreateTerminal
        public IActionResult CreateTerminal()
        {
            return View();
        }

        // POST: /Maritime/CreateTerminal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTerminal(Terminal terminal)
        {
            var user = await _userManager.GetUserAsync(User);


            if (ModelState.IsValid)
            {
                terminal.CompanyId = user.CompanyId!.Value;
                terminal.CreatedAt = DateTime.UtcNow;
                await _context.Terminals.AddAsync(terminal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(terminal);
        }

        // GET: /Maritime/EditTerminal/5
        public async Task<IActionResult> EditTerminal(int id)
        {
            var terminal = await _context.Terminals.FindAsync(id);
            if (terminal == null) return NotFound();
            return View(terminal);
        }

        // POST: /Maritime/EditTerminal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTerminal(int id, Terminal terminal)
        {
            if (id != terminal.Id) return NotFound();

            if (!ModelState.IsValid) return View(terminal);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                terminal.CompanyId = user.CompanyId.Value;
                terminal.UpdatedAt = DateTime.UtcNow;

                _context.Update(terminal);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(terminal);
            }
        }

        // POST: /Maritime/DeleteTerminal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTerminal(int id)
        {
            var entity = await _context.Terminals.FindAsync(id);
            if (entity == null) return NotFound();

            _context.Terminals.Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "Terminal" });
        }




        // GET: /Maritime/CreateLolo
        public IActionResult CreateLolo()
        {
            return View();
        }

        // POST: /Maritime/CreateLolo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLolo(Lolo lolo)
        {
            var user = await _userManager.GetUserAsync(User);


            if (ModelState.IsValid)
            {
                lolo.CompanyId = user.CompanyId!.Value;
                lolo.CreatedAt = DateTime.UtcNow;
                await _context.Lolos.AddAsync(lolo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lolo);
        }

        // GET: /Maritime/EditLolo/5
        public async Task<IActionResult> EditLolo(int id)
        {
            var lolo = await _context.Lolos.FindAsync(id);
            if (lolo == null) return NotFound();
            return View(lolo);
        }

        // POST: /Maritime/EditLolo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLolo(int id, Lolo lolo)
        {
            if (id != lolo.Id) return NotFound();

            if (!ModelState.IsValid) return View(lolo);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                lolo.CompanyId = user.CompanyId.Value;
                lolo.UpdatedAt = DateTime.UtcNow;

                _context.Update(lolo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(lolo);
            }
        }


        // POST: /Maritime/DeleteLolo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLolo(int id)
        {
            var entity = await _context.Lolos.FindAsync(id);
            if (entity == null) return NotFound();

            _context.Lolos.Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "Lolo" });
        }





        // GET: /Maritime/CreateShippingLine
        public IActionResult CreateShippingLine()
        {
            return View();
        }

        // POST: /Maritime/CreateShippingLine
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShippingLine(ShippingLine line)
        {
            var user = await _userManager.GetUserAsync(User);


            if (ModelState.IsValid)
            {
                line.CompanyId = user.CompanyId!.Value;
                line.CreatedAt = DateTime.UtcNow;
                await _context.ShippingLines.AddAsync(line);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(line);
        }

        // GET: /Maritime/EditShippingLine/5
        public async Task<IActionResult> EditShippingLine(int id)
        {
            var line = await _context.ShippingLines.FindAsync(id);
            if (line == null) return NotFound();
            return View(line);
        }

        // POST: /Maritime/EditShippingLine/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShippingLine(int id, ShippingLine line)
        {
            if (id != line.Id) return NotFound();

            if (!ModelState.IsValid) return View(line);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                line.CompanyId = user.CompanyId.Value;
                line.UpdatedAt = DateTime.UtcNow;

                _context.Update(line);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View(line);
            }
        }


        // POST: /Maritime/DeleteShippingLine/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShippingLine(int id)
        {
            var entity = await _context.ShippingLines.FindAsync(id);
            if (entity == null) return NotFound();

            _context.ShippingLines.Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "ShippingLine" });
        }

    }
}
