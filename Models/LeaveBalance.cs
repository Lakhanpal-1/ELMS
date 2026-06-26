namespace EmployeeLeaveManagementSystem.Models
{
    public class LeaveBalance
    {
        public int LeaveBalanceId { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }

        public int Year { get; set; }

        public int TotalDays { get; set; }

        public int UsedDays { get; set; }

        public int RemainingDays { get; set; }
    }
}
