using MediatR;

namespace Application.Features.Companies.DeleteCompany
{
    /// <summary>
    /// Command to delete a company by its ID.
    /// </summary>
    public class DeleteCompanyCommand : IRequest
    {
        /// <summary>
        /// The ID of the company.
        /// </summary>
        public Guid Id { get; set; }
    }
}