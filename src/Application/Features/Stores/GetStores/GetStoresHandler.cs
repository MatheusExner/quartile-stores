using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Stores.GetStores;

public class GetStoresHandler(IStoreRepository storeRepository, IMapper mapper) : IRequestHandler<GetStoresQuery, IEnumerable<StoreDto>>
{
    private readonly IStoreRepository _storeRepository = storeRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handles the retrieval of a store by its ID.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<IEnumerable<StoreDto>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
    {
        var stores = await _storeRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<StoreDto>>(stores.OrderBy(s => s.Name));
    }
}

