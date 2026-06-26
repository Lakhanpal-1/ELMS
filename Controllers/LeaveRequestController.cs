using System.Security.Claims;
using EmployeeLeaveManagementSystem.Models.ViewModels;
using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeLeaveManagementSystem.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly ILeaveBalanceService _leaveBalanceService;
        private readonly IEmployeeService _employeeService;

        public LeaveRequestController(
            ILeaveRequestService leaveRequestService,
            ILeaveTypeService leaveTypeService,
            ILeaveBalanceService leaveBalanceService,
            IEmployeeService employeeService)
        {
            _leaveRequestService = leaveRequestService;
            _leaveTypeService = leaveTypeService;
            _leaveBalanceService = leaveBalanceService;
            _employeeService = employeeService;
        }

        // Employee: own requests. Admin: every request.
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var all = await _leaveRequestService.GetAllAsync();
                return View(all);
            }

            var employee = await CurrentEmployeeAsync();
            if (employee == null)
                return Forbid();

            var own = await _leaveRequestService.GetByEmployeeIdAsync(employee.EmployeeId);
            return View(own);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ExportMyRequests()
        {
            var employee = await CurrentEmployeeAsync();
            if (employee == null) return Forbid();

            var requests = await _leaveRequestService.GetByEmployeeIdAsync(employee.EmployeeId);
            
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Leave Type,Start Date,End Date,Days,Status,Applied Date,Reason");
            
            foreach (var req in requests.OrderByDescending(r => r.AppliedDate))
            {
                var reasonEscaped = req.Reason?.Replace("\"", "\"\"") ?? "";
                builder.AppendLine($"\"{req.LeaveType?.Name}\",\"{req.StartDate:yyyy-MM-dd}\",\"{req.EndDate:yyyy-MM-dd}\",\"{req.NumberOfDays}\",\"{req.Status}\",\"{req.AppliedDate:yyyy-MM-dd}\",\"{reasonEscaped}\"");
            }
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            return File(bytes, "text/csv", $"LeaveHistory_{DateTime.Now:yyyyMMdd}.csv");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportAllRequests()
        {
            var requests = await _leaveRequestService.GetAllAsync();
            
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Employee Name,Department,Leave Type,Start Date,End Date,Days,Status,Applied Date,Reason");
            
            foreach (var req in requests.OrderByDescending(r => r.AppliedDate))
            {
                var reasonEscaped = req.Reason?.Replace("\"", "\"\"") ?? "";
                var empName = req.Employee?.FullName?.Replace("\"", "\"\"") ?? "Unknown";
                var deptName = req.Employee?.Department?.DepartmentName?.Replace("\"", "\"\"") ?? "Unknown";
                
                builder.AppendLine($"\"{empName}\",\"{deptName}\",\"{req.LeaveType?.Name}\",\"{req.StartDate:yyyy-MM-dd}\",\"{req.EndDate:yyyy-MM-dd}\",\"{req.NumberOfDays}\",\"{req.Status}\",\"{req.AppliedDate:yyyy-MM-dd}\",\"{reasonEscaped}\"");
            }
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(builder.ToString());
            return File(bytes, "text/csv", $"SystemLeaveReport_{DateTime.Now:yyyyMMdd}.csv");
        }

        public async Task<IActionResult> Details(int id)
        {
            var leaveRequest = await _leaveRequestService.GetByIdAsync(id);
            if (leaveRequest == null)
                return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var employee = await CurrentEmployeeAsync();
                if (employee == null || leaveRequest.EmployeeId != employee.EmployeeId)
                    return Forbid();
            }

            return View(leaveRequest);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Apply()
        {
            var employee = await CurrentEmployeeAsync();
            if (employee == null)
                return Forbid();

            var model = new LeaveRequestApplyViewModel
            {
                LeaveTypeOptions = await LeaveTypeOptions(),
                RemainingDaysByLeaveType = await RemainingDaysByLeaveType(employee.EmployeeId, DateTime.UtcNow.Year)
            };
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ApplyModalPartial", model);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(LeaveRequestApplyViewModel model)
        {
            var employee = await CurrentEmployeeAsync();
            if (employee == null)
                return Forbid();

            if (!ModelState.IsValid)
            {
                model.LeaveTypeOptions = await LeaveTypeOptions();
                model.RemainingDaysByLeaveType = await RemainingDaysByLeaveType(employee.EmployeeId, DateTime.UtcNow.Year);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_ApplyModalPartial", model);
                return View(model);
            }

            var result = await _leaveRequestService.ApplyAsync(employee.EmployeeId, model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);
                model.LeaveTypeOptions = await LeaveTypeOptions();
                model.RemainingDaysByLeaveType = await RemainingDaysByLeaveType(employee.EmployeeId, DateTime.UtcNow.Year);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_ApplyModalPartial", model);
                return View(model);
            }

            TempData["Success"] = "Leave request submitted.";
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var employee = await CurrentEmployeeAsync();
            if (employee == null)
                return Forbid();

            var result = await _leaveRequestService.CancelAsync(id, employee.EmployeeId);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? "Leave request cancelled."
                : string.Join(" ", result.Errors);

            return RedirectToAction(nameof(Index));
        }

        // Admin: queue of pending requests awaiting a decision.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var pending = await _leaveRequestService.GetPendingAsync();
            return View(pending);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? remarks)
        {
            var result = await _leaveRequestService.ApproveAsync(id, CurrentUserId(), remarks);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? "Leave request approved."
                : string.Join(" ", result.Errors);

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && referer.Contains("/Dashboard", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Dashboard");
            
            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? remarks)
        {
            var result = await _leaveRequestService.RejectAsync(id, CurrentUserId(), remarks);
            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? "Leave request rejected."
                : string.Join(" ", result.Errors);

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && referer.Contains("/Dashboard", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Dashboard");

            return RedirectToAction(nameof(Pending));
        }

        private string CurrentUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        private Task<Models.Employee?> CurrentEmployeeAsync() =>
            _employeeService.GetByApplicationUserIdAsync(CurrentUserId());

        private async Task<List<SelectListItem>> LeaveTypeOptions()
        {
            var leaveTypes = await _leaveTypeService.GetAllAsync();
            return leaveTypes
                .Select(t => new SelectListItem($"{t.Name} ({t.DefaultDaysPerYear} days/yr)", t.LeaveTypeId.ToString()))
                .ToList();
        }

        private async Task<Dictionary<int, int>> RemainingDaysByLeaveType(int employeeId, int year)
        {
            var leaveTypes = await _leaveTypeService.GetAllAsync();
            var map = new Dictionary<int, int>();
            foreach (var type in leaveTypes)
                map[type.LeaveTypeId] = await _leaveBalanceService.GetRemainingDaysAsync(employeeId, type.LeaveTypeId, year);

            return map;
        }
    }
}
