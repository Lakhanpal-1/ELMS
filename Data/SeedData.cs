using EmployeeLeaveManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Data
{
    /// <summary>
    /// Runs once at application startup to guarantee the system is usable
    /// the moment the database is created — roles exist, there is one Admin
    /// login to sign in with, and a couple of starter lookups exist so the
    /// "Apply for Leave" / "Add Employee" forms aren't empty on first run.
    /// </summary>
    public static class SeedData
    {
        public const string AdminEmail = "admin@screenhive.local";
        public const string AdminPassword = "Admin@123";

        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var roleName in new[] { "Admin", "Employee" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if (await userManager.FindByEmailAsync(AdminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    FullName = "HR Administrator",
                    EmailConfirmed = true
                };

                var created = await userManager.CreateAsync(admin, AdminPassword);
                if (created.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (!await context.Departments.AnyAsync())
            {
                context.Departments.AddRange(
                    new Department { DepartmentName = "Engineering" },
                    new Department { DepartmentName = "Human Resources" },
                    new Department { DepartmentName = "Sales" }
                );
            }

            if (!await context.LeaveTypes.AnyAsync())
            {
                context.LeaveTypes.AddRange(
                    new LeaveType { Name = "Casual Leave", DefaultDaysPerYear = 12 },
                    new LeaveType { Name = "Sick Leave", DefaultDaysPerYear = 10 },
                    new LeaveType { Name = "Earned Leave", DefaultDaysPerYear = 15 }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
