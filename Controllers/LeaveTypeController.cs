using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LeaveTypeController : Controller
    {
        private readonly ILeaveTypeService _leaveTypeService;

        public LeaveTypeController(ILeaveTypeService leaveTypeService)
        {
            _leaveTypeService = leaveTypeService;
        }

        public async Task<IActionResult> Index()
        {
            var leaveTypes = await _leaveTypeService.GetAllAsync();
            return View(leaveTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new LeaveType());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveType leaveType)
        {
            if (!ModelState.IsValid)
                return View(leaveType);

            var result = await _leaveTypeService.CreateAsync(leaveType);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(leaveType);
            }

            TempData["Success"] = "Leave type created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var leaveType = await _leaveTypeService.GetByIdAsync(id);
            if (leaveType == null)
                return NotFound();

            return View(leaveType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveType leaveType)
        {
            if (id != leaveType.LeaveTypeId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(leaveType);

            var result = await _leaveTypeService.UpdateAsync(leaveType);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(leaveType);
            }

            TempData["Success"] = "Leave type updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var leaveType = await _leaveTypeService.GetByIdAsync(id);
            if (leaveType == null)
                return NotFound();

            return View(leaveType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _leaveTypeService.DeleteAsync(id);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" ", result.Errors);
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Leave type deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
