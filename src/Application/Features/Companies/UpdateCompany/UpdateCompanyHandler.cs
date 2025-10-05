using System.Net;
using Application.Common;
using Application.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Companies.UpdateCompany;

public class UpdateCompanyHandler(ICompanyRepository companyRepository, IMapper mapper, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository = companyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    /// <summary>
    /// Handles the update of an existing company.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<CompanyDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(request.Id);
        if (existingCompany == null)
        {
            throw new CustomException("Company not found.", HttpStatusCode.NotFound);
        }

        var companyWithSameName = await _companyRepository.GetCompanyByNameAsync(request.Name);
        if (companyWithSameName != null && companyWithSameName.Id != request.Id)
        {
            throw new CustomException("Another company with the same name already exists.", HttpStatusCode.UnprocessableEntity);
        }

        existingCompany.Name = request.Name;

        await _companyRepository.UpdateAsync(existingCompany);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CompanyDto>(existingCompany);
    }
}

