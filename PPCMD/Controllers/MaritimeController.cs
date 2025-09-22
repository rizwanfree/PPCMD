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

        // GET: Create form
        public IActionResult Create(string type)
        {
            if (string.IsNullOrEmpty(type))
                return BadRequest("Type is required.");

            ViewBag.Type = type;

            Maritime model = type.ToLower() switch
            {
                "terminal" => new Terminal(),
                "lolo" => new Lolo(),
                "shippingline" => new ShippingLine(),
                _ => throw new ArgumentException("Invalid type")
            };

            return View("Create", model);
            
        }

        // POST: Create new record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string type, Maritime model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Type = type;
                return View("Create", model);
            }

            model.CreatedAt = DateTime.UtcNow;

            switch (type.ToLower())
            {
                case "terminal":
                    _context.Terminals.Add((Terminal)model);
                    break;
                case "lolo":
                    _context.Lolos.Add((Lolo)model);
                    break;
                case "shippingline":
                    _context.ShippingLines.Add((ShippingLine)model);
                    break;
                default:
                    return BadRequest("Invalid type.");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { type });
        }

        // GET: Edit form
        public async Task<IActionResult> Edit(string type, int id)
        {
            if (string.IsNullOrEmpty(type))
                return BadRequest("Type is required.");

            ViewBag.Type = type;

            Maritime? entity = type.ToLower() switch
            {
                "terminal" => await _context.Terminals.FindAsync(id),
                "lolo" => await _context.Lolos.FindAsync(id),
                "shippingline" => await _context.ShippingLines.FindAsync(id),
                _ => null
            };

            if (entity == null) return NotFound();

            return View("Edit", entity);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string type, int id, Maritime model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Type = type;
                return View("Edit", model);
            }

            model.UpdatedAt = DateTime.UtcNow;

            switch (type.ToLower())
            {
                case "terminal":
                    _context.Terminals.Update((Terminal)model);
                    break;
                case "lolo":
                    _context.Lolos.Update((Lolo)model);
                    break;
                case "shippingline":
                    _context.ShippingLines.Update((ShippingLine)model);
                    break;
                default:
                    return BadRequest("Invalid type.");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { type });
        }

        // Delete
        public async Task<IActionResult> Delete(string type, int id)
        {
            if (string.IsNullOrEmpty(type))
                return BadRequest("Type is required.");

            Maritime? entity = type.ToLower() switch
            {
                "terminal" => await _context.Terminals.FindAsync(id),
                "lolo" => await _context.Lolos.FindAsync(id),
                "shippingline" => await _context.ShippingLines.FindAsync(id),
                _ => null
            };

            if (entity == null) return NotFound();

            switch (type.ToLower())
            {
                case "terminal": _context.Terminals.Remove((Terminal)entity); break;
                case "lolo": _context.Lolos.Remove((Lolo)entity); break;
                case "shippingline": _context.ShippingLines.Remove((ShippingLine)entity); break;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { type });
        }



    }
}
