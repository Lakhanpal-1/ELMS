using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.ViewModels;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByApplicationUserIdAsync(string applicationUserId);

        /// <summary>Creates the Identity login (role = Employee) and the linked Employee profile row.</summary>
        Task<ServiceResult> CreateAsync(EmployeeCreateViewModel model);

        Task<ServiceResult> UpdateAsync(EmployeeEditViewModel model);

        /// <summary>Removes the Employee profile and its Identity login.</summary>
        Task<ServiceResult> DeleteAsync(int id);
    }
}
