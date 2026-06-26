using EmployeeLeaveManagementSystem.Models;

namespace EmployeeLeaveManagementSystem.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        /// <summary>Employee row linked to the currently logged-in Identity user.</summary>
        Task<Employee?> GetByApplicationUserIdAsync(string applicationUserId);

        /// <summary>Single employee including Department + ApplicationUser navigation.</summary>
        Task<Employee?> GetWithDetailsAsync(int employeeId);

        /// <summary>All employees including Department, ordered by name.</summary>
        Task<List<Employee>> GetAllWithDetailsAsync();

        Task<bool> EmailExistsAsync(string email);
    }
}
