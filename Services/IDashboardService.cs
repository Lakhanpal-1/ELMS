using EmployeeLeaveManagementSystem.Models.ViewModels;

namespace EmployeeLeaveManagementSystem.Services
{
    public interface IDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardAsync();
        Task<EmployeeDashboardViewModel?> GetEmployeeDashboardAsync(int employeeId);
    }
}
