using System.Net;
using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Stores.GetStore;

public class GetStoreHandler(IStoreRepository storeRepository, IMapper mapper) : IRequestHandler<GetStoreQuery, DetailedStoreDto>
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
    public async Task<DetailedStoreDto> Handle(GetStoreQuery request, CancellationToken cancellationToken)
    {
        var existingStore = await _storeRepository.GetByIdAsync(request.Id);

        if (existingStore == null)
        {
            throw new CustomException("Store not found.", HttpStatusCode.NotFound);
        }

        return _mapper.Map<DetailedStoreDto>(existingStore);
    }
}

