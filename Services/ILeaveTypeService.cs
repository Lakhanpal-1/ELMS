using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface ILeaveTypeService
    {
        Task<List<LeaveType>> GetAllAsync();
        Task<LeaveType?> GetByIdAsync(int id);
        Task<ServiceResult> CreateAsync(LeaveType leaveType);
        Task<ServiceResult> UpdateAsync(LeaveType leaveType);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
