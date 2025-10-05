using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        protected readonly SqlServerDataContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(SqlServerDataContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public Task UpdateAsync(T entity)
        {
            return Task.FromResult(_dbSet.Update(entity));
        }

        public Task DeleteAsync(T entity)
        {
            return Task.FromResult(_dbSet.Remove(entity));
        }

        public async Task DeleteByIdAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}
