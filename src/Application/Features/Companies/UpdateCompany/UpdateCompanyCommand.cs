using Application.Dtos;
using MediatR;

namespace Application.Features.Companies.UpdateCompany
{
    /// <summary>
    /// Command to update an existing company.
    /// </summary>
    public class UpdateCompanyCommand : IRequest<CompanyDto>
    {
        /// <summary>
        /// The ID of the company.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the company.
        /// </summary>
        public string Name { get; set; } = null!;
    }
}