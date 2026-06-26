namespace EmployeeLeaveManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Designation { get; set; }

        public DateTime DateOfJoining { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        public ICollection<LeaveRequest>? LeaveRequests { get; set; }
        public ICollection<LeaveBalance>? LeaveBalances { get; set; }
    }
}
