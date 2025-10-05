using Application.Dtos;
using MediatR;

namespace Application.Features.Stores.CreateStore
{
    /// <summary>
    /// Command to create a new store.
    /// </summary>
    public class CreateStoreCommand : IRequest<StoreDto>
    {
        /// <summary>
        /// The name of the store.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The address of the store.
        /// </summary>
        public string Address { get; set; } = null!;

        /// <summary>
        /// The city where the store is located.
        /// </summary>
        public string City { get; set; } = null!;

        /// <summary>
        /// The country where the store is located.
        /// </summary>
        public string Country { get; set; } = null!;

        /// <summary>
        /// The ID of the company that owns the store.
        /// </summary>
        public Guid CompanyId { get; set; }
    }
}