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



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.CompanyId == null)
                {
                    ModelState.AddModelError(string.Empty, "User not authenticated");
                    return await PopulateViewAndReturn();
                }

                // Extract form data, validate, save to DB, etc.

                // Step 1
                var jobNumber = form["PendingBL.JobNumber"].ToString();
                var jobDateStr = form["PendingBL.JobDate"].ToString();
                var clientIdStr = form["PendingBL.ClientId"].ToString();

                if (string.IsNullOrWhiteSpace(jobNumber))
                    ModelState.AddModelError("PendingBL.JobNumber", "Job Number is required");

                if (string.IsNullOrWhiteSpace(jobDateStr) || !DateTime.TryParse(jobDateStr, out DateTime jobDate))
                    ModelState.AddModelError("PendingBL.JobDate", "Valid job date is required.");

                if (string.IsNullOrWhiteSpace(clientIdStr) || !int.TryParse(clientIdStr, out int clientId))
                    ModelState.AddModelError("PendingBL.ClientId", "Client selection is required.");


                // Step 2

                // Get IGM From Form. If not found, create new IGM record

                var igmNumberStr = form["PendingBL.IGM.Number"].ToString();
                var igmDateStr = form["PendingBL.IGM.Date"].ToString();
                var igmPortIdStr = form["PendingBL.IGM.PortId"].ToString();
                var vessel = form["PendingBL.IGM.Vessel"].ToString();

                // verify IGM Details
                var igm = await _context.IGMs.Where(i => i.CompanyId == user.CompanyId.Value &&
                               i.PortId.ToString() == igmPortIdStr &&
                               i.Number.ToString() == igmNumberStr &&
                               i.Date.ToString("yyyy-MM-dd") == igmDateStr)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (igm == null)
                {
                    Console.WriteLine("IGM not found during form submission");

                    // Add New IGM to Table

                    IGM newIgm = new IGM
                    {
                        CompanyId = user.CompanyId.Value,
                        PortId = int.Parse(igmPortIdStr),
                        Number = int.Parse(igmNumberStr),
                        Date = DateTime.Parse(igmDateStr),
                        Vessel = vessel.ToUpper()
                    };
                    Console.WriteLine("Adding new IGM to database");
                    Console.WriteLine($"IGM Details: CompanyId={newIgm.CompanyId}, PortId={newIgm.PortId}, Number={newIgm.Number}, Date={newIgm.Date}, Vessel={newIgm.Vessel}");

                    igm = newIgm;
                }

                // Step 3
                // Extract BL Details and Add to PendingBL

                var blNumber = form["BL.Number"].ToString();
                var blDateStr = form["BL.Date"].ToString();
                var indexNumberStr = form["BL.IndexNumber"].ToString();
                var quantityStr = "0";
                var AssignedQuantityStr = quantityStr;

                if (string.IsNullOrWhiteSpace(blNumber))
                    ModelState.AddModelError("BL.Number", "BL Number is required");
                if (string.IsNullOrWhiteSpace(blDateStr) || !DateTime.TryParse(blDateStr, out DateTime blDate))
                    ModelState.AddModelError("BL.Date", "Valid BL date is required.");

                if (string.IsNullOrWhiteSpace(indexNumberStr) || !int.TryParse(indexNumberStr, out int indexNumber))
                    ModelState.AddModelError("BL.IndexNumber", "Valid Index Number is required.");


                PendingBL pendingBL = new PendingBL
                {
                    CompanyId = user.CompanyId.Value,
                    ClientId = int.Parse(clientIdStr),
                    JobNumber = int.Parse(jobNumber),
                    JobDate = DateTime.Parse(jobDateStr),
                    IGM = igm,
                    BLNumber = blNumber.ToUpper(),
                    BLDate = DateTime.Parse(blDateStr),
                    IndexNumber = int.Parse(indexNumberStr),
                    Quantity = decimal.Parse(quantityStr),
                    AssignedQuantity = decimal.Parse(AssignedQuantityStr)
                };


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                
            }
            // Handle form submission logic here
            // Extract form data, validate, save to DB, etc.
            // For now, just redirect back to Index
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> PopulateViewAndReturn()
        {
            throw new NotImplementedException();
        }









        // JSON Data Endpoints for AJAX calls

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

                // Debug logging
                Console.WriteLine($"Looking up IGM: Number={igmNumber}, PortId={portId}, Year={year}, CompanyId={user.CompanyId.Value}");

                // Find IGM for current company, port, year, and number
                var igm = await _context.IGMs
                    .Where(i => i.CompanyId == user.CompanyId.Value &&
                               i.PortId == portId &&
                               i.Number == igmNumber &&
                               i.Date.Year == year)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (igm == null)
                {
                    Console.WriteLine("IGM not found");
                    return Json(new { success = false, message = "IGM not found" });
                }

                Console.WriteLine($"IGM found: Vessel={igm.Vessel}, Date={igm.Date}");

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
                Console.WriteLine($"IGM lookup error: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult GetShippingLineDetails(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user?.CompanyId == null) return Json(new { success = false });

            var sl = _context.ShippingLines
                .Where(s => s.Id == id)
                .AsNoTracking()
                .FirstOrDefault();

            if (sl == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                name = sl.Name,
                ntn = sl.NTN,                
                phone = sl.Phone
            });
        }

        [HttpGet]
        public IActionResult GetLoloDetails(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user?.CompanyId == null) return Json(new { success = false });

            var lolo = _context.Lolos
                .Where(l => l.Id == id)
                .AsNoTracking()
                .FirstOrDefault();

            if (lolo == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                name = lolo.Name,
                ntn = lolo.NTN
            });
        }

        [HttpGet]
        public IActionResult GetTerminalDetails(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user?.CompanyId == null) return Json(new { success = false });

            var terminal = _context.Terminals
                .Where(t => t.Id == id)
                .AsNoTracking()
                .FirstOrDefault();

            if (terminal == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                name = terminal.Name,
                ntn = terminal.NTN
            });
        }
    }
}
