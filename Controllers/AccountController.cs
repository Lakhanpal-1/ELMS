using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.ViewModels;
using EmployeeLeaveManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction("Index", "Dashboard");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var departments = await _departmentService.GetAllAsync();
            return View(new RegisterViewModel { Departments = departments });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = await _departmentService.GetAllAsync();
                return View(model);
            }

            var createResult = await _employeeService.CreateAsync(new EmployeeCreateViewModel
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Designation = model.Designation,
                DateOfJoining = model.DateOfJoining,
                DepartmentId = model.DepartmentId
            });

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error);

                model.Departments = await _departmentService.GetAllAsync();
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
                await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var model = new SettingsViewModel
            {
                FullName = user.FullName ?? user.UserName?.Split('@')[0] ?? "User",
                Email = user.Email ?? string.Empty
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.Email))
            {
                TempData["Error"] = "Full Name and Email cannot be empty.";
                return RedirectToAction("Settings");
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            
            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                var employee = await _employeeService.GetByApplicationUserIdAsync(user.Id);
                if (employee != null)
                {
                    employee.FullName = model.FullName;
                    employee.Email = model.Email;
                }
                TempData["Success"] = "Profile details updated successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to update profile details. Email may already be in use.";
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Settings");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword) || model.NewPassword != model.ConfirmNewPassword)
            {
                TempData["Error"] = "Please provide valid password details. Passwords must match.";
                return RedirectToAction("Settings");
            }

            var pwdResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!pwdResult.Succeeded)
            {
                TempData["Error"] = string.Join(" ", pwdResult.Errors.Select(e => e.Description));
                return RedirectToAction("Settings");
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Password updated successfully.";
            return RedirectToAction("Settings");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
