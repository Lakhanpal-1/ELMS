namespace EmployeeLeaveManagementSystem.Models
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DefaultDaysPerYear { get; set; }

        public ICollection<LeaveRequest>? LeaveRequests { get; set; }
        public ICollection<LeaveBalance>? LeaveBalances { get; set; }
    }
}
