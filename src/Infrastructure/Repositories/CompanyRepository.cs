using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CompanyRepository(SqlServerDataContext context) : Repository<Company>(context), ICompanyRepository
    {
        private readonly DbSet<Company> _dbSet = context.Set<Company>();

        /// <summary>
        /// Gets a company by its name.
        /// </summary>
        /// <param name="name">The name of the company.</param>
        /// <returns></returns>
        public Task<Company> GetCompanyByNameAsync(string name)
        {
            return _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}