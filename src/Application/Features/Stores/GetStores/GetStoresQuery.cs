using Application.Dtos;
using MediatR;

namespace Application.Features.Stores.GetStores
{
    /// <summary>
    /// Query to retrieve all stores.
    /// </summary>
    public class GetStoresQuery : IRequest<IEnumerable<StoreDto>>
    {
    }
}