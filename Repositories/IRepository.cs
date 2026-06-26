using System.Linq.Expressions;

namespace EmployeeLeaveManagementSystem.Repositories
{
    /// <summary>
    /// Generic data-access contract shared by every entity repository.
    /// Keeps EF Core (DbContext/DbSet) out of the Service layer.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> SaveChangesAsync();
    }
}
