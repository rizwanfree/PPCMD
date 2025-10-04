using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPCMD.Data;
using PPCMD.Models;

namespace PPCMD.Controllers
{
    public class HeaderController : BaseController
    {
        public HeaderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            // Get DutyTypes
            var dutyTypes = await _context.DutyTypes
                .Where(d => d.CompanyId == user.CompanyId.Value)
                .AsNoTracking()
                .ToListAsync();

            // Get PayorderHeaders
            var headers = await _context.PayorderHeaders
                .Where(h => h.CompanyId == user.CompanyId.Value)
                .AsNoTracking()
                .ToListAsync();

            // Pass both into ViewBag
            ViewBag.DutyTypes = dutyTypes;
            ViewBag.PayorderHeaders = headers;

            // Just return empty model, since we’re using ViewBag for both
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SaveDutyType([FromBody] DutyType model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (model.Id == 0) // Create
                {
                    model.CompanyId = user.CompanyId.Value;
                    _context.DutyTypes.Add(model);
                }
                else // Update
                {
                    var existing = await _context.DutyTypes
                        .Where(d => d.Id == model.Id && d.CompanyId == user.CompanyId.Value)
                        .FirstOrDefaultAsync();

                    if (existing == null)
                        return NotFound();

                    existing.Name = model.Name;
                    existing.Description = model.Description;
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SavePayorderHeader([FromBody] PayorderHeader model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (model.Id == 0) // Create
                {
                    model.CompanyId = user.CompanyId.Value;
                    _context.PayorderHeaders.Add(model);
                }
                else // Update
                {
                    var existing = await _context.PayorderHeaders
                        .Where(h => h.Id == model.Id && h.CompanyId == user.CompanyId.Value)
                        .FirstOrDefaultAsync();

                    if (existing == null)
                        return NotFound();

                    existing.Name = model.Name;
                    existing.Description = model.Description;
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteDutyType(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            try
            {
                var dutyType = await _context.DutyTypes
                    .Where(d => d.Id == id && d.CompanyId == user.CompanyId.Value)
                    .FirstOrDefaultAsync();

                if (dutyType == null)
                    return NotFound();

                _context.DutyTypes.Remove(dutyType);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeletePayorderHeader(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.CompanyId == null) return Forbid();

            try
            {
                var header = await _context.PayorderHeaders
                    .Where(h => h.Id == id && h.CompanyId == user.CompanyId.Value)
                    .FirstOrDefaultAsync();

                if (header == null)
                    return NotFound();

                _context.PayorderHeaders.Remove(header);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
