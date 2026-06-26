using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public interface ILeaveBalanceRepository : IRepository<LeaveBalance>
    {
        Task<LeaveBalance?> GetAsync(int employeeId, int leaveTypeId, int year);

        Task<List<LeaveBalance>> GetByEmployeeIdAsync(int employeeId, int year);
    }
}
