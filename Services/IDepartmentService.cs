using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(Department department);
        Task<ServiceResult> UpdateAsync(Department department);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
