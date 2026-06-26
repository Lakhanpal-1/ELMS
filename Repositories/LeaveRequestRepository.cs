using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        private IQueryable<LeaveRequest> WithDetails() =>
            DbSet
                .Include(r => r.Employee)
                    .ThenInclude(e => e!.Department)
                .Include(r => r.LeaveType)
                .Include(r => r.ApprovedBy);

        public async Task<LeaveRequest?> GetWithDetailsAsync(int leaveRequestId) =>
            await WithDetails().FirstOrDefaultAsync(r => r.LeaveRequestId == leaveRequestId);

        public async Task<List<LeaveRequest>> GetByEmployeeIdAsync(int employeeId) =>
            await WithDetails()
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.AppliedDate)
                .ToListAsync();

        public async Task<List<LeaveRequest>> GetPendingAsync() =>
            await WithDetails()
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.AppliedDate)
                .ToListAsync();

        public async Task<List<LeaveRequest>> GetAllWithDetailsAsync() =>
            await WithDetails()
                .OrderByDescending(r => r.AppliedDate)
                .ToListAsync();

        public async Task<bool> HasOverlappingRequestAsync(int employeeId, DateTime startDate, DateTime endDate, int? excludeLeaveRequestId = null) =>
            await DbSet.AnyAsync(r =>
                r.EmployeeId == employeeId &&
                r.Status != "Rejected" &&
                (excludeLeaveRequestId == null || r.LeaveRequestId != excludeLeaveRequestId) &&
                r.StartDate <= endDate &&
                r.EndDate >= startDate);
    }
}
