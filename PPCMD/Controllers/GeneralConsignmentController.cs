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
        public IActionResult Create()
        {
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
        public async Task<JsonResult> GetItemDuties(int itemId)
        {
            var duties = await _context.ItemDuties
                .Where(id => id.ItemID == itemId)
                .Include(id => id.DutyType)
                .OrderBy(id => id.Order)
                .Select(id => new
                {
                    dutyTypeId = id.DutyTypeId,
                    dutyTypeName = id.DutyType.Name,
                    rate = id.Rate,
                    isPercentage = id.IsPercentage,
                    order = id.Order
                })
                .ToListAsync();

            return Json(duties);
        }
    }
}
