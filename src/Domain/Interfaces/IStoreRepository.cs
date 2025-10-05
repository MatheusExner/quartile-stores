using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IStoreRepository : IRepository<Store>
    {   
        /// <summary>
        /// Gets a store by its ID, including related company data.
        /// </summary>
        /// <param name="id">The ID of the store.</param>
        /// <returns></returns>
        Task<Store> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a store by its name.
        /// </summary>
        /// <param name="name">The name of the store.</param>
        /// <returns></returns>
        Task<Store> GetStoreByNameAsync(string name);
    }
}