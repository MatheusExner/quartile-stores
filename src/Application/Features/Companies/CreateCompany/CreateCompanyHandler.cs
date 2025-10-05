using System.Net;
using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Companies.CreateCompany;

public class CreateCompanyHandler(ICompanyRepository companyRepository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
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
    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var existingCompany = await _companyRepository.GetCompanyByNameAsync(request.Name);
        if (existingCompany != null)
        {
            throw new CustomException("Company with the same name already exists.", HttpStatusCode.UnprocessableEntity);
        }

        var company = new Company
        {
            Name = request.Name
        };

        await _companyRepository.AddAsync(company);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CompanyDto>(company);
    }
}

