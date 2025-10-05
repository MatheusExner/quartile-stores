using System.Net;
using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Companies.GetCompany;

public class GetCompanyHandler(ICompanyRepository companyRepository, IMapper mapper) : IRequestHandler<GetCompanyQuery, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository = companyRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handles the retrieval of a company by its ID.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<CompanyDto> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(request.Id);

        if (existingCompany == null)
        {
            throw new CustomException("Company not found.", HttpStatusCode.NotFound);
        }

        return _mapper.Map<CompanyDto>(existingCompany);
    }
}

