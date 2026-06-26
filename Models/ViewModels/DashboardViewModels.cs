using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalLeaveTypes { get; set; }
        public int PendingRequestsCount { get; set; }
        public int ApprovedThisMonthCount { get; set; }
        public List<LeaveRequest> RecentPendingRequests { get; set; } = new();
        public List<LeaveRequest> RecentApplications { get; set; } = new();
        public List<LeaveRequest> CompanyLeavesOnToday { get; set; } = new();
        public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
    }

    public class EmployeeDashboardViewModel
    {
        public Employee Employee { get; set; } = null!;
        public List<LeaveBalance> Balances { get; set; } = new();
        public List<LeaveRequest> RecentRequests { get; set; } = new();
        public List<LeaveRequest> TeamLeavesOnToday { get; set; } = new();
    }
}
