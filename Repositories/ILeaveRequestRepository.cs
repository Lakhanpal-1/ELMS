using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<LeaveRequest?> GetWithDetailsAsync(int leaveRequestId);

        Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);

        Task<List<LeaveRequest>> GetPendingAsync();

        Task<List<LeaveRequest>> GetAllWithDetailsAsync();

        /// <summary>
        /// True if the employee already has a non-rejected leave request
        /// whose date range overlaps the given range.
        /// </summary>
        Task<bool> HasOverlappingRequestAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveRequestId = null);
    }
}
