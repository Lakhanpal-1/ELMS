using EmployeeLeaveManagementSystem.Models;
using EmployeeLeaveManagementSystem.Models.ViewModels;
using EmployeeLeaveManagementSystem.Repositories;

namespace EmployeeLeaveManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<LeaveType> _leaveTypeRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeDetailRepository;
        private readonly ILeaveBalanceService _leaveBalanceService;

        public DashboardService(
            IRepository<Employee> employeeRepository,
            IRepository<Department> departmentRepository,
            IRepository<LeaveType> leaveTypeRepository,
            ILeaveRequestRepository leaveRequestRepository,
            IEmployeeRepository employeeDetailRepository,
            ILeaveBalanceService leaveBalanceService)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveRequestRepository = leaveRequestRepository;
            _employeeDetailRepository = employeeDetailRepository;
            _leaveBalanceService = leaveBalanceService;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var employees = await _employeeDetailRepository.GetAllWithDetailsAsync();
            var departments = await _departmentRepository.GetAllAsync();
            var leaveTypes = await _leaveTypeRepository.GetAllAsync();
            var pending = await _leaveRequestRepository.GetPendingAsync();
            var all = await _leaveRequestRepository.GetAllWithDetailsAsync();

            var now = DateTime.UtcNow;
            var today = now.Date;

            var approvedThisMonth = all.Count(r =>
                r.Status == "Approved" && r.ApprovedDate.HasValue &&
                r.ApprovedDate.Value.Month == now.Month && r.ApprovedDate.Value.Year == now.Year);

            var companyLeavesOnToday = all
                .Where(r => r.Status == "Approved" && r.StartDate.Date <= today && r.EndDate.Date >= today)
                .ToList();

            var employeesByDept = employees
                .GroupBy(e => string.IsNullOrWhiteSpace(e.Department?.DepartmentName) ? "Unassigned" : e.Department.DepartmentName)
                .ToDictionary(g => g.Key, g => g.Count());

            var recentApps = all
                .Where(r => r.AppliedDate >= today.AddDays(-7))
                .ToList();

            return new AdminDashboardViewModel
            {
                TotalEmployees = employees.Count,
                TotalDepartments = departments.Count,
                TotalLeaveTypes = leaveTypes.Count,
                PendingRequestsCount = pending.Count,
                ApprovedThisMonthCount = approvedThisMonth,
                RecentPendingRequests = pending.OrderByDescending(r => r.AppliedDate).Take(10).ToList(),
                RecentApplications = recentApps,
                CompanyLeavesOnToday = companyLeavesOnToday,
                EmployeesByDepartment = employeesByDept
            };
        }

        public async Task<EmployeeDashboardViewModel?> GetEmployeeDashboardAsync(int employeeId)
        {
            var employee = await _employeeDetailRepository.GetWithDetailsAsync(employeeId);
            if (employee == null)
                return null;

            var balances = await _leaveBalanceService.GetBalancesForEmployeeAsync(employeeId, DateTime.UtcNow.Year);
            var requests = await _leaveRequestRepository.GetByEmployeeIdAsync(employeeId);

            var allRequests = await _leaveRequestRepository.GetAllWithDetailsAsync();
            var today = DateTime.UtcNow.Date;
            var teamLeaves = allRequests
                .Where(r => r.Employee != null &&
                            r.Employee.DepartmentId == employee.DepartmentId &&
                            r.EmployeeId != employeeId &&
                            r.Status == "Approved" &&
                            r.StartDate.Date <= today && r.EndDate.Date >= today)
                .ToList();

            return new EmployeeDashboardViewModel
            {
                Employee = employee,
                Balances = balances,
                RecentRequests = requests.Take(5).ToList(),
                TeamLeavesOnToday = teamLeaves
            };
        }
    }
}
