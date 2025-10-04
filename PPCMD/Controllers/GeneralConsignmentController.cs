using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class GeneralConsignmentController : BaseController
    {

        public GeneralConsignmentController(ApplicationDbContext context,UserManager<ApplicationUser> userManager) 
            : base(context, userManager) { }


        // GET: /GeneralConsignment/
        public IActionResult Index()
        {
            var bls = _context.BLs
                .Include(b => b.PendingBL)
                .ThenInclude(pb => pb.Client)
                .Include(b => b.PendingBL)
                .ThenInclude(pb => pb.Items)
                .ThenInclude(bi => bi.Item)
                .Include(b => b.LC)
                .Include(b => b.Company)
                .AsNoTracking()
                .ToList();

            return View(bls);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            // Populate ViewBag with dropdown data
            ViewBag.Clients = new SelectList(_context.Clients, "Id", "ClientName");
            ViewBag.Ports = new SelectList(_context.Ports, "Id", "Name");
            ViewBag.ShippingLines = new SelectList(_context.ShippingLines, "Id", "Name");
            ViewBag.Terminals = new SelectList(_context.Terminals, "Id", "Name");
            ViewBag.Lolos = new SelectList(_context.Lolos, "Id", "Name");
            ViewBag.Items = new SelectList(_context.Items, "Id", "ItemName");
            ViewBag.ConsignmentType = new List<SelectListItem>
            {
                new SelectListItem { Text = "Home Consumption", Value = "Home Consumption" },
                new SelectListItem { Text = "Into-Bond", Value = "Into-Bond" },
                new SelectListItem { Text = "Safe Transportation", Value = "Safe Transportation" },
                new SelectListItem { Text = "Trans-Shipment", Value = "Trans-Shipment" }
            };

            // Add PayorderHeaders to ViewBag
            ViewBag.PayorderHeaders = _context.PayorderHeaders
                .Where(h => h.CompanyId == user.CompanyId.Value)
                .OrderBy(h => h.Name)
                .ToList();

            // Pre-load all items with their duties
            var itemsWithDuties = _context.Items
                .Include(i => i.Duties)
                    .ThenInclude(d => d.DutyType)
                .Select(i => new {
                    i.Id,
                    i.ItemName,
                    Duties = i.Duties.Select(d => new {
                        d.DutyTypeId,
                        DutyTypeName = d.DutyType.Name,
                        d.Rate,
                        d.IsPercentage,
                        d.Order
                    }).OrderBy(d => d.Order).ToList()
                })
                .ToList();

            ViewBag.ItemsWithDuties = itemsWithDuties;

            return View();
        }




        [HttpGet]
        public IActionResult GetClientContact(int clientId)
        {
            var client = _context.Clients
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == clientId);

            if (client == null)
                return Json(new { success = false });

            return Json(new { success = true, contactPerson = client.ContactPerson });
        }


        [HttpGet]
        public async Task<IActionResult> GetIGMDetails(int igmNumber, int portId, int year)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.CompanyId == null)
                    return Json(new { success = false, message = "User not authenticated" });

                // Find IGM for current company, port, year, and number
                var igm = await _context.IGMs
                    .Where(i => i.CompanyId == user.CompanyId.Value &&
                               i.PortId == portId &&
                               i.Number == igmNumber &&
                               i.Date.Year == year)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (igm == null)
                    return Json(new { success = false, message = "IGM not found" });

                return Json(new
                {
                    success = true,
                    igmNumber = igm.Number,
                    igmDate = igm.Date.ToString("yyyy-MM-dd"),
                    vessel = igm.Vessel,
                    portId = igm.PortId
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
