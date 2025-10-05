using Application.Dtos;
using MediatR;

namespace Application.Features.Companies.GetCompany
{
    /// <summary>
    /// Query to get a company by its ID.
    /// </summary>
    public class GetCompanyQuery : IRequest<CompanyDto>
    {
        /// <summary>
        /// The ID of the company.
        /// </summary>
        public Guid Id { get; set; }
    }
}