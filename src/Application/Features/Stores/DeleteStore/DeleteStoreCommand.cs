using MediatR;

namespace Application.Features.Stores.DeleteStore
{
    /// <summary>
    /// Command to delete a store by its ID.
    /// </summary>
    public class DeleteStoreCommand : IRequest
    {
        /// <summary>
        /// The ID of the store.
        /// </summary>
        public Guid Id { get; set; }
    }
}