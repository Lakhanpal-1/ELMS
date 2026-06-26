using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IEmployeeService _employeeService;

        public DashboardController(IDashboardService dashboardService, IEmployeeService employeeService)
        {
            _dashboardService = dashboardService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var adminModel = await _dashboardService.GetAdminDashboardAsync();
                return View("Admin", adminModel);
            }

            var userId = _userId();
            var employee = await _employeeService.GetByApplicationUserIdAsync(userId);
            if (employee == null)
            {
                TempData["Error"] = "No employee profile is linked to your account yet. Please contact HR.";
                return View("Employee", null);
            }

            var employeeModel = await _dashboardService.GetEmployeeDashboardAsync(employee.EmployeeId);
            return View("Employee", employeeModel);
        }

        private string _userId() =>
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }
}
