using System.Net;
using Application.Common;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Companies.DeleteCompany;

public class DeleteCompanyHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository = companyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Handles the deletion of a company by its ID.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(request.Id);

        if (existingCompany == null)
        {
            throw new CustomException("Company not found.", HttpStatusCode.NotFound);
        }
        await _companyRepository.DeleteAsync(existingCompany);
        await _unitOfWork.SaveChangesAsync();
    }
}
