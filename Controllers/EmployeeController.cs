using EmployeeLeaveManagementSystem.Models.ViewModels;
using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeLeaveManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllAsync();
            return View(employees);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeCreateViewModel
            {
                DepartmentOptions = await DepartmentOptions()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DepartmentOptions = await DepartmentOptions();
                return View(model);
            }

            var result = await _employeeService.CreateAsync(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);
                model.DepartmentOptions = await DepartmentOptions();
                return View(model);
            }

            TempData["Success"] = "Employee created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            var model = new EmployeeEditViewModel
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Email = employee.Email,
                Designation = employee.Designation,
                DateOfJoining = employee.DateOfJoining,
                DepartmentId = employee.DepartmentId,
                DepartmentOptions = await DepartmentOptions()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel model)
        {
            if (id != model.EmployeeId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.DepartmentOptions = await DepartmentOptions();
                return View(model);
            }

            var result = await _employeeService.UpdateAsync(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);
                model.DepartmentOptions = await DepartmentOptions();
                return View(model);
            }

            TempData["Success"] = "Employee updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _employeeService.DeleteAsync(id);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" ", result.Errors);
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Employee removed.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> DepartmentOptions()
        {
            var departments = await _departmentService.GetAllAsync();
            return departments
                .Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString()))
                .ToList();
        }
    }
}
