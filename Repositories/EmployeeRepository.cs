using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetByApplicationUserIdAsync(string applicationUserId) =>
            await DbSet
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);

        public async Task<Employee?> GetWithDetailsAsync(int employeeId) =>
            await DbSet
                .Include(e => e.Department)
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        public async Task<List<Employee>> GetAllWithDetailsAsync() =>
            await DbSet
                .Include(e => e.Department)
                .OrderBy(e => e.FullName)
                .ToListAsync();

        public async Task<bool> EmailExistsAsync(string email) =>
            await DbSet.AnyAsync(e => e.Email.ToLower() == email.ToLower());
    }
}
