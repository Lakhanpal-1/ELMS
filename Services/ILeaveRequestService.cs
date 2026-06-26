using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.ViewModels;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
        Task<List<LeaveRequest>> GetPendingAsync();
        Task<List<LeaveRequest>> GetAllAsync();

        Task<ServiceResult> ApplyAsync(int employeeId, LeaveRequestApplyViewModel model);

        Task<ServiceResult> ApproveAsync(int leaveRequestId, string approvedByUserId, string? remarks);

        Task<ServiceResult> RejectAsync(int leaveRequestId, string approvedByUserId, string? remarks);

        /// <summary>Employee withdraws their own request while it is still Pending.</summary>
        Task<ServiceResult> CancelAsync(int leaveRequestId, int employeeId);
    }
}
