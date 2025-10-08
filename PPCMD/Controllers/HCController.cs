using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class HCController : BaseController
    {

        public HCController(ApplicationDbContext context,UserManager<ApplicationUser> userManager) 
            : base(context, userManager) { }


        // GET: /GeneralConsignment/
        public IActionResult Index()
        {
            var consignments = _context.Consignments
                .Include(c => c.LC)
                .ThenInclude(lc => lc.BLs)
                .ThenInclude(b => b.PendingBL)
                .ThenInclude(pb => pb.Client)
                .Include(c => c.Company)
                .Where(c => c.ConsignmentTitle == "Home Consumption")
                .AsNoTracking()
                .ToList();

            return View(consignments);
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


        // CEATE: /HC/Create
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

                _context.SetTenant(user.CompanyId.Value);

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Step 1: Save IGM (only if new)
                    Console.WriteLine("Step 1: Processing IGM...");
                    var igm = await ExtractAndValidateIGM(form, user);
                    if (!ModelState.IsValid) return await PopulateViewAndReturn();

                    // ✅ The ExtractAndValidateIGM method now handles saving internally for new IGMs
                    Console.WriteLine($"IGM Processed - ID: {igm.Id}, Number: {igm.Number}");



                    // Step 2: Save LC
                    Console.WriteLine("Step 2: Saving LC...");
                    var (blItems, blQuantity) = ExtractBLItems(form, user);
                    var lc = await ExtractAndValidateLC(form, blQuantity, user);
                    if (!ModelState.IsValid) return await PopulateViewAndReturn();

                    await _context.LCs.AddAsync(lc);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"LC Saved - ID: {lc.Id}, Number: {lc.LCNumber}");



                    // Step 3: Save PendingBL
                    Console.WriteLine("Step 3: Saving PendingBL...");
                    var pendingBL = ExtractAndValidatePendingBL(form, user, igm);
                    if (!ModelState.IsValid) return await PopulateViewAndReturn();

                    pendingBL.BLQuantity = blQuantity;
                    pendingBL.AssignedQuantity = blQuantity;
                    //pendingBL.IGMId = igm.Id;


                    await _context.PendingBLs.AddAsync(pendingBL);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"PendingBL Saved - ID: {pendingBL.Id}, BL Number: {pendingBL.BLNumber}");



                    // Step 4: Save BL
                    Console.WriteLine("Step 4: Saving BL...");
                    var bl = ExtractBL(form, lc, user);
                    if (!ModelState.IsValid) return await PopulateViewAndReturn();

                    bl.LCId = lc.Id; // Set foreign key
                    bl.PendingBLId = pendingBL.Id; // Set foreign key

                    await _context.BLs.AddAsync(bl);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"BL Saved - ID: {bl.Id}");



                    // Step 5: Update PendingBL with BL relationship
                    Console.WriteLine("Step 5: Updating PendingBL with BL...");
                    pendingBL.BL = bl;
                    _context.PendingBLs.Update(pendingBL);
                    await _context.SaveChangesAsync();




                    // Step 6: Save BLItems with their DutyCharges
                    Console.WriteLine("Step 6: Saving BLItems with DutyCharges...");

                    var itemIndex = 0;

                    foreach (var item in blItems)
                    {
                        // Save BLItem first
                        item.PendingBLId = pendingBL.Id;
                        await _context.BLItems.AddAsync(item);
                        await _context.SaveChangesAsync(); // Save to get the BLItem ID

                        Console.WriteLine($"BLItem Saved - ID: {item.Id}, ItemId: {item.ItemId}");

                        // Now extract and save DutyCharges for this specific item
                        var dutyCharges = ExtractDutyChargesForItem(form, itemIndex, user, item.Id);
                        foreach (var dutyCharge in dutyCharges)
                        {
                            dutyCharge.BLItemId = item.Id; // Link to the BLItem we just saved
                            await _context.DutyCharges.AddAsync(dutyCharge);
                            Console.WriteLine($"DutyCharge Saved - TypeId: {dutyCharge.DutyTypeId}, Amount: {dutyCharge.Amount}");
                        }

                        await _context.SaveChangesAsync(); // Save DutyCharges for this item
                        Console.WriteLine($"DutyCharges Saved for BLItem {item.Id} - Count: {dutyCharges.Count}");

                        itemIndex++;
                    }
                    Console.WriteLine($"All BLItems and DutyCharges Saved - Total Items: {blItems.Count}");



                    // Step 7: Save Payorders
                    Console.WriteLine("Step 8: Saving Payorders...");
                    var payorders = ExtractPayorders(form, user);
                    foreach (var payorder in payorders)
                    {
                        payorder.BLId = bl.Id; // Set foreign key
                        payorder.LCId = lc.Id; // Set foreign key
                        await _context.Payorders.AddAsync(payorder);
                    }
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Payorders Saved - Count: {payorders.Count}");

                    // Step 8
                    // : Save Consignment
                    Console.WriteLine("Step 9: Saving Consignment...");
                    var consignment = CreateConsignment(lc);
                    

                    await _context.Consignments.AddAsync(consignment);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Consignment Saved - ID: {consignment.Id}, Title: {consignment.ConsignmentTitle}");

                    await transaction.CommitAsync();

                    Console.WriteLine("✅ ALL DATA SAVED SUCCESSFULLY!");
                    TempData["Success"] = "Consignment created successfully!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"❌ Transaction Rolled Back: {ex.Message}");
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                TempData["Error"] = $"Error creating consignment: {ex.Message}";
                return RedirectToAction(nameof(Create));
            }
        }

        private async Task<IActionResult> PopulateViewAndReturn()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            // Repopulate all the ViewBag data that was in the GET Create method
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

            // Return the view with the current ModelState (which contains validation errors)
            return View();
        }



        // Edit: /HC/Edit/5

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            var consignment = await _context.Consignments
                .Include(c => c.LC)                    // Get the LC
                    .ThenInclude(lc => lc.BLs)         // Then get BLs from that LC
                        .ThenInclude(b => b.PendingBL) // Then get PendingBL from each BL
                            .ThenInclude(pb => pb.IGM) // Then get IGM from PendingBL
                .Include(c => c.LC)
                    .ThenInclude(lc => lc.BLs)
                        .ThenInclude(b => b.PendingBL)
                            .ThenInclude(pb => pb.Items) // Then get Items from PendingBL
                                .ThenInclude(bi => bi.Item) // Then get Item details
                .Include(c => c.LC)
                    .ThenInclude(lc => lc.BLs)
                        .ThenInclude(b => b.PendingBL)
                            .ThenInclude(pb => pb.Items)
                                .ThenInclude(bi => bi.DutyCharges) // Then get DutyCharges
                .Include(c => c.LC)
                    .ThenInclude(lc => lc.BLs)
                        .ThenInclude(b => b.Payorders) // Then get Payorders from each BL
                .FirstOrDefaultAsync(c => c.Id == id && c.CompanyId == user.CompanyId.Value);

            if (consignment == null)
            {
                return NotFound();
            }

            // Populate ViewBag (same as Create)
            await PopulateViewBag(user);
    
            return View(consignment);
        }


        // Helper Methods
        private async Task<IGM> ExtractAndValidateIGM(IFormCollection form, ApplicationUser user)
        {
            var igmNumber = form["PendingBL.IGM.Number"].ToString();
            var igmDate = form["PendingBL.IGM.Date"].ToString();
            var igmVessel = form["PendingBL.IGM.Vessel"].ToString();
            var igmPortId = form["PendingBL.IGM.PortId"].ToString();

            // Validate required fields
            if (string.IsNullOrEmpty(igmNumber))
                ModelState.AddModelError("PendingBL.IGM.Number", "IGM Number is required");
            if (string.IsNullOrEmpty(igmDate))
                ModelState.AddModelError("PendingBL.IGM.Date", "IGM Date is required");
            if (string.IsNullOrEmpty(igmPortId))
                ModelState.AddModelError("PendingBL.IGM.PortId", "Port is required");

            if (!ModelState.IsValid) return null!;

            // Parse the date first
            if (!DateTime.TryParse(igmDate, out DateTime parsedIgmDate))
            {
                ModelState.AddModelError("PendingBL.IGM.Date", "Invalid IGM Date format");
                return null!;
            }

            // Check if IGM already exists
            var igm = await _context.IGMs
                .Where(i => i.CompanyId == user.CompanyId!.Value &&
                           i.PortId == int.Parse(igmPortId) &&
                           i.Number == int.Parse(igmNumber) &&
                           i.Date.Date == parsedIgmDate.Date)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (igm == null)
            {
                Console.WriteLine("IGM not found during form submission - creating new IGM");

                igm = new IGM
                {
                    Number = int.Parse(igmNumber),
                    Date = parsedIgmDate,
                    Vessel = igmVessel.ToUpper(),
                    PortId = int.Parse(igmPortId),
                    CompanyId = user.CompanyId!.Value
                };
                Console.WriteLine($"New IGM created: CompanyId={igm.CompanyId}, PortId={igm.PortId}, Number={igm.Number}, Date={igm.Date}, Vessel={igm.Vessel}");

                // ✅ Only save if it's a NEW IGM
                await _context.IGMs.AddAsync(igm);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Existing IGM found: ID={igm.Id}, Number={igm.Number}");
                // ✅ Don't save existing IGM - just use its ID
            }

            return igm;
        }

        private PendingBL ExtractAndValidatePendingBL(IFormCollection form, ApplicationUser user, IGM igm)
        {
            var jobNumber = form["PendingBL.JobNumber"].ToString();
            var jobDate = form["PendingBL.JobDate"].ToString();
            var clientId = form["PendingBL.ClientId"].ToString();
            var blNumber = form["PendingBL.BLNumber"].ToString();
            var blDate = form["PendingBL.BLDate"].ToString();
            var indexNumber = form["PendingBL.IndexNumber"].ToString();

            // Validate required fields
            if (string.IsNullOrEmpty(jobNumber))
                ModelState.AddModelError("PendingBL.JobNumber", "Job Number is required");
            if (string.IsNullOrEmpty(jobDate))
                ModelState.AddModelError("PendingBL.JobDate", "Job Date is required");
            if (string.IsNullOrEmpty(clientId))
                ModelState.AddModelError("PendingBL.ClientId", "Client is required");
            if (string.IsNullOrEmpty(blNumber))
                ModelState.AddModelError("PendingBL.BLNumber", "BL Number is required");
            if (string.IsNullOrEmpty(blDate))
                ModelState.AddModelError("PendingBL.BLDate", "BL Date is required");
            if (string.IsNullOrEmpty(indexNumber))
                ModelState.AddModelError("PendingBL.IndexNumber", "Index Number is required");

            if (!ModelState.IsValid) return null!;

            return new PendingBL
            {
                JobNumber = int.Parse(jobNumber),
                JobDate = DateTime.Parse(jobDate),
                ClientId = int.Parse(clientId),
                BLNumber = blNumber.ToUpper(),
                BLDate = DateTime.Parse(blDate),
                IndexNumber = int.Parse(indexNumber),
                CompanyId = user.CompanyId!.Value,
                IGMId = igm.Id
            };
        }

        private (List<BLItem> items, decimal totalQuantity) ExtractBLItems(IFormCollection form, ApplicationUser user)
        {
            var items = new List<BLItem>();
            var itemIndex = 0;
            decimal totalQuantity = 0;

            while (true)
            {
                var itemId = form[$"Items[{itemIndex}].ItemId"].ToString();
                if (string.IsNullOrEmpty(itemId)) break;

                var blItem = ExtractBLItem(form, itemIndex, user);

                // DEBUG: Check the BLItem IMMEDIATELY after creation
                Console.WriteLine($"BLItem {itemIndex} - IMMEDIATELY AFTER CREATION - Id: {blItem.Id}, ItemId: {blItem.ItemId}");

                if (blItem.Id != 0)
                {
                    Console.WriteLine($"❌ PROBLEM FOUND: BLItem Id is {blItem.Id} right after creation!");

                    // Force reset the Id
                    var itemType = blItem.GetType();
                    var idProperty = itemType.GetProperty("Id");
                    if (idProperty != null && idProperty.CanWrite)
                    {
                        idProperty.SetValue(blItem, 0);
                        Console.WriteLine($"✅ Reset BLItem Id from {blItem.Id} to 0");
                    }
                }

                items.Add(blItem);
                totalQuantity += blItem.Quantity;
                itemIndex++;
            }

            return (items, totalQuantity);
        }

        private BLItem ExtractBLItem(IFormCollection form, int itemIndex, ApplicationUser user)
        {
            var quantity = form[$"Items[{itemIndex}].Quantity"].ToString();
            var unitValue = form[$"Items[{itemIndex}].UnitValue"].ToString();
            var invoiceValue = form[$"Items[{itemIndex}].ImportValue"].ToString();
            var invoiceValuePKR = form[$"Items[{itemIndex}].InvoiceValuePKR"].ToString();
            var insuranceUSD = form[$"Items[{itemIndex}].InsuranceValue"].ToString();
            var insurancePKR = form[$"Items[{itemIndex}].InsuranceValuePKR"].ToString();
            var landingCharges = form[$"Items[{itemIndex}].LandingCharges"].ToString();
            var assessableValue = form[$"Items[{itemIndex}].AssessableValue"].ToString();

            return new BLItem
            {
                
                ItemId = int.Parse(form[$"Items[{itemIndex}].ItemId"].ToString()), // This is the foreign key to Item, not the primary key
                Quantity = decimal.Parse(quantity),
                DeclaredValue = string.IsNullOrEmpty(unitValue) ? 0 : decimal.Parse(unitValue),
                InvoiceValue = string.IsNullOrEmpty(invoiceValue) ? null : decimal.Parse(invoiceValue),
                InvoiceValuePKR = string.IsNullOrEmpty(invoiceValuePKR) ? null : decimal.Parse(invoiceValuePKR),
                InsuranceUSD = string.IsNullOrEmpty(insuranceUSD) ? null : decimal.Parse(insuranceUSD),
                InsurancePKR = string.IsNullOrEmpty(insurancePKR) ? null : decimal.Parse(insurancePKR),
                LandingCharges = string.IsNullOrEmpty(landingCharges) ? null : decimal.Parse(landingCharges),
                AssessableValue = string.IsNullOrEmpty(assessableValue) ? null : decimal.Parse(assessableValue),
                CompanyId = user.CompanyId!.Value
            };
        }


        private List<DutyCharge> ExtractDutyChargesForItem(IFormCollection form, int itemIndex, ApplicationUser user, int blItemId)
        {
            var dutyCharges = new List<DutyCharge>();
            var dutyIndex = 0;

            while (true)
            {
                var dutyTypeId = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].DutyTypeId"].ToString();
                if (string.IsNullOrEmpty(dutyTypeId)) break;

                var rate = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].Rate"].ToString();
                var isPercentage = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].IsPercentage"].ToString();
                var amount = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].Amount"].ToString();

                var dutyCharge = new DutyCharge
                {
                    BLItemId = blItemId, // This will be set properly
                    DutyTypeId = int.Parse(dutyTypeId),
                    Rate = decimal.Parse(rate),
                    IsPercentage = bool.Parse(isPercentage),
                    Amount = string.IsNullOrEmpty(amount) ? 0 : int.Parse(amount),
                    CompanyId = user.CompanyId!.Value
                };

                dutyCharges.Add(dutyCharge);
                dutyIndex++;
            }

            return dutyCharges;
        }


        private async Task<LC> ExtractAndValidateLC(IFormCollection form, decimal blQuantity, ApplicationUser user)
        {
            var lcNumber = form["PendingBL.BL.LC.LCNumber"].ToString();
            var lcDate = form["PendingBL.BL.LC.Date"].ToString();

            if (string.IsNullOrEmpty(lcNumber))
                ModelState.AddModelError("PendingBL.BL.LC.LCNumber", "LC Number is required");
            if (string.IsNullOrEmpty(lcDate))
                ModelState.AddModelError("PendingBL.BL.LC.Date", "LC Date is required");

            if (!ModelState.IsValid) return null!;

            // Check if LC already exists
            var existingLC = await _context.LCs
                .FirstOrDefaultAsync(l => l.CompanyId == user.CompanyId!.Value &&
                                         l.LCNumber == lcNumber);

            if (existingLC != null)
            {
                existingLC.TotalQuantity += blQuantity;
                existingLC.UpdatedAt = DateTime.UtcNow;
                return existingLC;
            }

            return new LC
            {
                LCNumber = lcNumber,
                Date = DateTime.Parse(lcDate),
                TotalQuantity = blQuantity,
                CompanyId = user.CompanyId!.Value
            };
        }

        private BL ExtractBL(IFormCollection form, LC lc, ApplicationUser user)
        {
            var containers = form["PendingBL.BL.Containers"].ToString();
            var size = form["PendingBL.BL.Size"].ToString();
            var packages = form["PendingBL.BL.Packages"].ToString();
            var exchangeRate = form["PendingBL.BL.ExchangeRate"].ToString();
            var shippingLineId = form["PendingBL.BL.ShippingLineId"].ToString();
            var terminalId = form["PendingBL.BL.TerminalId"].ToString();
            var loloId = form["PendingBL.BL.LoloId"].ToString();

            // Validate exchange rate (required for calculations)
            if (string.IsNullOrEmpty(exchangeRate))
                ModelState.AddModelError("PendingBL.BL.ExchangeRate", "Exchange Rate is required");

            if (!ModelState.IsValid) return null!;

            return new BL
            {
                Containers = string.IsNullOrEmpty(containers) ? null : int.Parse(containers),
                Size = string.IsNullOrEmpty(size) ? null : int.Parse(size),
                Packages = packages,
                ExchangeRate = decimal.Parse(exchangeRate),
                ShippingLineId = string.IsNullOrEmpty(shippingLineId) ? null : int.Parse(shippingLineId),
                TerminalId = string.IsNullOrEmpty(terminalId) ? null : int.Parse(terminalId),
                LoloId = string.IsNullOrEmpty(loloId) ? null : int.Parse(loloId),
                LC = lc,
                CompanyId = user.CompanyId!.Value
            };
        }

        private List<DutyCharge> ExtractDutyCharges(IFormCollection form, ApplicationUser user)
        {
            var dutyCharges = new List<DutyCharge>();
            var itemIndex = 0;

            while (true)
            {
                var itemId = form[$"Items[{itemIndex}].ItemId"].ToString();
                if (string.IsNullOrEmpty(itemId)) break;

                ExtractItemDutyCharges(form, itemIndex, dutyCharges, user);
                itemIndex++;
            }

            return dutyCharges;
        }

        private void ExtractItemDutyCharges(IFormCollection form, int itemIndex, List<DutyCharge> dutyCharges, ApplicationUser user)
        {
            var dutyIndex = 0;

            while (true)
            {
                var dutyTypeId = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].DutyTypeId"].ToString();
                if (string.IsNullOrEmpty(dutyTypeId)) break;

                var rate = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].Rate"].ToString();
                var isPercentage = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].IsPercentage"].ToString();
                var amount = form[$"Items[{itemIndex}].DutyCalculations[{dutyIndex}].Amount"].ToString();

                var dutyCharge = new DutyCharge
                {
                    DutyTypeId = int.Parse(dutyTypeId),
                    Rate = decimal.Parse(rate),
                    IsPercentage = bool.Parse(isPercentage),
                    Amount = string.IsNullOrEmpty(amount) ? 0 : int.Parse(amount),
                    CompanyId = user.CompanyId!.Value
                };

                dutyCharges.Add(dutyCharge);
                dutyIndex++;
            }
        }

        private List<Payorder> ExtractPayorders(IFormCollection form, ApplicationUser user)
        {
            var payorders = new List<Payorder>();
            var payorderIndex = 0;

            Console.WriteLine("Starting payorder extraction...");

            while (true)
            {
                // Get the particular text directly from the form
                var particular = form[$"Payorders[{payorderIndex}].Particular"].ToString();

                Console.WriteLine($"Checking payorder index {payorderIndex}: Particular='{particular}'");

                if (string.IsNullOrEmpty(particular))
                {
                    Console.WriteLine($"No payorder found at index {payorderIndex} - stopping extraction");
                    break;
                }

                var amount = form[$"Payorders[{payorderIndex}].Amount"].ToString();
                var detail = form[$"Payorders[{payorderIndex}].Detail"].ToString();
                var order = form[$"Payorders[{payorderIndex}].Order"].ToString();

                Console.WriteLine($"  Amount: {amount}, Detail: {detail}, Order: {order}");

                // Only create payorder if amount is greater than 0
                if (!string.IsNullOrEmpty(amount) && decimal.Parse(amount) > 0)
                {
                    var payorder = new Payorder
                    {
                        Particular = particular, // Use the text from the form
                        Amount = decimal.Parse(amount),
                        Detail = detail,
                        Order = string.IsNullOrEmpty(order) ? payorderIndex : int.Parse(order),
                        CompanyId = user.CompanyId!.Value
                    };

                    payorders.Add(payorder);
                    Console.WriteLine($"  ✅ Added payorder: {particular}");
                }
                else
                {
                    Console.WriteLine($"  ⚠️ Skipped payorder (amount 0 or empty): {particular}");
                }

                payorderIndex++;
            }

            Console.WriteLine($"Payorder extraction complete - Found: {payorders.Count} payorders");
            return payorders;
        }

        private Consignment CreateConsignment(LC lc)
        {
            return new Consignment
            {
                ConsignmentTitle = "Home Consumption",
                LCId = lc.Id,
                IsSelf = false,
                CompanyId = lc.CompanyId // Use same company ID as LC
            };
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
