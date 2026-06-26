using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.ViewModels;
using EmployeeLeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EmployeeLeaveManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public const string EmployeeRoleName = "Employee";

        public EmployeeService(IEmployeeRepository employeeRepository, UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
        }

        public async Task<List<Employee>> GetAllAsync() => await _employeeRepository.GetAllWithDetailsAsync();

        public async Task<Employee?> GetByIdAsync(int id) => await _employeeRepository.GetWithDetailsAsync(id);

        public async Task<Employee?> GetByApplicationUserIdAsync(string applicationUserId) =>
            await _employeeRepository.GetByApplicationUserIdAsync(applicationUserId);

        public async Task<ServiceResult> CreateAsync(EmployeeCreateViewModel model)
        {
            if (await _employeeRepository.EmailExistsAsync(model.Email))
                return ServiceResult.Failure("An employee with this email already exists.");

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return ServiceResult.Failure("A login already exists for this email.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
                return ServiceResult.Failure(createResult.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, EmployeeRoleName);

            var employee = new Employee
            {
                FullName = model.FullName,
                Email = model.Email,
                Designation = model.Designation,
                DateOfJoining = model.DateOfJoining,
                DepartmentId = model.DepartmentId,
                ApplicationUserId = user.Id
            };

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateAsync(EmployeeEditViewModel model)
        {
            var employee = await _employeeRepository.GetByIdAsync(model.EmployeeId);
            if (employee == null)
                return ServiceResult.Failure("Employee not found.");

            var emailOwner = await _employeeRepository.FindAsync(e =>
                e.EmployeeId != model.EmployeeId && e.Email.ToLower() == model.Email.Trim().ToLower());
            if (emailOwner.Count > 0)
                return ServiceResult.Failure("Another employee already uses this email.");

            employee.FullName = model.FullName.Trim();
            employee.Email = model.Email.Trim();
            employee.Designation = model.Designation;
            employee.DateOfJoining = model.DateOfJoining;
            employee.DepartmentId = model.DepartmentId;
            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();

            if (!string.IsNullOrEmpty(employee.ApplicationUserId))
            {
                var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
                if (user != null)
                {
                    user.FullName = employee.FullName;
                    user.Email = employee.Email;
                    user.UserName = employee.Email;
                    await _userManager.UpdateAsync(user);
                }
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return ServiceResult.Failure("Employee not found.");

            var applicationUserId = employee.ApplicationUserId;

            _employeeRepository.Remove(employee);
            await _employeeRepository.SaveChangesAsync();

            if (!string.IsNullOrEmpty(applicationUserId))
            {
                var user = await _userManager.FindByIdAsync(applicationUserId);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            return ServiceResult.Success();
        }
    }
}
