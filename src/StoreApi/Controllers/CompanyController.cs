using Application.Dtos;
using Application.Features.Companies.CreateCompany;
using Application.Features.Companies.DeleteCompany;
using Application.Features.Companies.GetCompanies;
using Application.Features.Companies.GetCompany;
using Application.Features.Companies.UpdateCompany;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompanyController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Creates a new company.
        /// </summary>
        /// <param name="name">The name of the company.</param>
        /// <param name="address">The address of the company.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateCompany), result);
        }

        /// <summary>
        /// Updates an existing company.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <param name="command">The command containing the updated company information.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Gets a company by its ID.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var result = await _mediator.Send(new GetCompanyQuery { Id = id });
            return Ok(result);
        }


        /// <summary>
        /// Get all companies.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
        public async Task<IActionResult> GetAllCompanies()
        {
            var result = await _mediator.Send(new GetCompaniesQuery());
            return Ok(result);
        }


        /// <summary>
        /// Deletes a company by its ID.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _mediator.Send(new DeleteCompanyCommand { Id = id });
            return NoContent();
        }
    }
}