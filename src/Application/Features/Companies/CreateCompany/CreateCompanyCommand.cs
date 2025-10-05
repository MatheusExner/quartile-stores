using Application.Dtos;
using MediatR;

namespace Application.Features.Companies.CreateCompany
{
    /// <summary>
    /// Command to create a new company.
    /// </summary>
    public class CreateCompanyCommand : IRequest<CompanyDto>
    {
        /// <summary>
        /// The name of the company.
        /// </summary>
        public string Name { get; set; } = null!;
    }
}