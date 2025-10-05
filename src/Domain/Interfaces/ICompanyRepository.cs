using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICompanyRepository : IRepository<Company>
    {
        /// <summary>
        /// Gets a company by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Company> GetCompanyByNameAsync(string name);
    }
}