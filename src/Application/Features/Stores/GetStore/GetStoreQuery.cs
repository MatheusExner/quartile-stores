using Application.Dtos;
using MediatR;

namespace Application.Features.Stores.GetStore
{
    /// <summary>
    /// Query to get a store by its ID.
    /// </summary>
    public class GetStoreQuery : IRequest<DetailedStoreDto>
    {
        /// <summary>
        /// The ID of the store.
        /// </summary>
        public Guid Id { get; set; }
    }
}