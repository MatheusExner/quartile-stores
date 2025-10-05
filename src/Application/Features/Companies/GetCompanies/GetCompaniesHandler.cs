using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Companies.GetCompanies;

public class GetCompaniesHandler(ICompanyRepository companyRepository, IMapper mapper) : IRequestHandler<GetCompaniesQuery, IEnumerable<CompanyDto>>
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
    public async Task<IEnumerable<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<CompanyDto>>(companies.OrderBy(c => c.Name));
    }
}

