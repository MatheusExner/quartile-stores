using System.Net;
using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Stores.UpdateStore;

public class UpdateStoreHandler(IStoreRepository storeRepository, ICompanyRepository companyRepository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<UpdateStoreCommand, StoreDto>
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
    public async Task<StoreDto> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
    {
        var existingStore = await _storeRepository.GetByIdAsync(request.Id);
        if (existingStore == null)
        {
            throw new CustomException("Store not found.", HttpStatusCode.NotFound);
        }

        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
        {
            throw new CustomException("Company not found.", HttpStatusCode.NotFound);
        }

        existingStore.Name = request.Name;
        existingStore.Address = request.Address;
        existingStore.City = request.City;
        existingStore.Country = request.Country;
        existingStore.CompanyId = company.Id;

        await _storeRepository.UpdateAsync(existingStore);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StoreDto>(existingStore);
    }
}

