using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public class LeaveBalanceRepository : Repository<LeaveBalance>, ILeaveBalanceRepository
    {
        public LeaveBalanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LeaveBalance?> GetAsync(int employeeId, int leaveTypeId, int year) =>
            await DbSet.FirstOrDefaultAsync(b =>
                b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId && b.Year == year);

        public async Task<List<LeaveBalance>> GetByEmployeeIdAsync(int employeeId, int year) =>
            await DbSet
                .Include(b => b.LeaveType)
                .Where(b => b.EmployeeId == employeeId && b.Year == year)
                .OrderBy(b => b.LeaveType!.Name)
                .ToListAsync();
    }
}
