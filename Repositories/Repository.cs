using System.Linq.Expressions;
using EmployeeLeaveManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IRepository{T}"/>.
    /// Handles the plain CRUD plumbing; entity-specific queries (joins,
    /// includes, lookups) live in the dedicated repositories that inherit
    /// from this class.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public async Task<List<T>> GetAllAsync() => await DbSet.ToListAsync();

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

        public void Update(T entity) => DbSet.Update(entity);

        public void Remove(T entity) => DbSet.Remove(entity);

        public async Task<bool> SaveChangesAsync() => await Context.SaveChangesAsync() > 0;
    }
}
