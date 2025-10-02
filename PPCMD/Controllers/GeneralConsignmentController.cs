using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                .Include(b => b.LC)
                .Include(b => b.Company)
                .AsNoTracking()
                .ToList();

            return View(bls);
        }
    }
}
