using System.Net;
using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Stores.CreateStore;

public class CreateStoreHandler(IStoreRepository storeRepository, ICompanyRepository companyRepository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<CreateStoreCommand, StoreDto>
{
    private readonly IStoreRepository _storeRepository = storeRepository;
    private readonly ICompanyRepository _companyRepository = companyRepository;

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    /// <summary>
    /// Handles the creation of a new company.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<StoreDto> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        var existingStore = await _storeRepository.GetStoreByNameAsync(request.Name);
        if (existingStore != null)
        {
            throw new CustomException("Store with the same name already exists.", HttpStatusCode.UnprocessableEntity);
        }

        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
        {
            throw new CustomException("Company not found.", HttpStatusCode.NotFound);
        }

        var store = new Store
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            CompanyId = company.Id
        };

        await _storeRepository.AddAsync(store);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StoreDto>(store);
    }
}

