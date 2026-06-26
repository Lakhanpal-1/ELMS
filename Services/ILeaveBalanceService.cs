using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface ILeaveBalanceService
    {
        /// <summary>Returns the employee's balance row for this leave type/year, auto-provisioning
        /// it from LeaveType.DefaultDaysPerYear the first time it's needed.</summary>
        Task<LeaveBalance> GetOrCreateBalanceAsync(int employeeId, int leaveTypeId, int year);

        Task<List<LeaveBalance>> GetBalancesForEmployeeAsync(int employeeId, int year);

        Task<int> GetRemainingDaysAsync(int employeeId, int leaveTypeId, int year);

        /// <summary>Increases UsedDays (e.g. on approval). Does not save — caller controls the transaction.</summary>
        Task ApplyUsageAsync(int employeeId, int leaveTypeId, int year, int days);

        /// <summary>Decreases UsedDays (e.g. when an approved request is later reversed). Does not save.</summary>
        Task ReleaseUsageAsync(int employeeId, int leaveTypeId, int year, int days);

        Task SaveChangesAsync();
    }
}
