using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPCMD.Models
{
    public class ApplicationUser : IdentityUser
    {
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    // Link back to Employee record
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    }
}

