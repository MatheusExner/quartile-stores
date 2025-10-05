using Application.Dtos;
using MediatR;

namespace Application.Features.Companies.GetCompanies
{
    /// <summary>
    /// Query to retrieve all companies.
    /// </summary>
    public class GetCompaniesQuery : IRequest<IEnumerable<CompanyDto>>
    {
    }
}