using Microsoft.AspNetCore.Identity;

namespace EmployeeLeaveManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
