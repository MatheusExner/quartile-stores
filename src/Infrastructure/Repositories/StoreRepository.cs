using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StoreRepository(SqlServerDataContext context) : Repository<Store>(context), IStoreRepository
    {
        private readonly DbSet<Store> _dbSet = context.Set<Store>();

        /// <summary>
        /// Gets a store by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<Store> GetStoreByNameAsync(string name)
        {
            return _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        /// <summary>
        /// Gets a store by its ID, including related company data.
        /// </summary>
        /// <param name="id">The ID of the store.</param>
        /// <returns></returns>
        public Task<Store> GetByIdAsync(Guid id)
        {
            return _dbSet
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}