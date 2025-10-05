using Application.Dtos;
using Application.Features.Companies.CreateCompany;
using Application.Features.Companies.DeleteCompany;
using Application.Features.Companies.GetCompanies;
using Application.Features.Companies.GetCompany;
using Application.Features.Companies.UpdateCompany;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StoreApi.Controllers;

namespace tests.Controllers
{
    public class CompanyControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CompanyController _controller;

        public CompanyControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CompanyController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateCompany_ReturnsCreatedAtAction_WithCompanyDto()
        {
            var command = new CreateCompanyCommand { Name = "Test company" };
            var companyDto = new CompanyDto { Id = Guid.NewGuid(), Name = "Test company" };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(companyDto);

            var result = await _controller.CreateCompany(command);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(companyDto, createdResult.Value);
        }

        [Fact]
        public async Task UpdateCompany_ReturnsOk_WithCompanyDto()
        {
            var id = Guid.NewGuid();
            var command = new UpdateCompanyCommand { Name = "Company 123" };
            var companyDto = new CompanyDto { Id = id, Name = "Company 123" };

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateCompanyCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companyDto);

            var result = await _controller.UpdateCompany(id, command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(companyDto, okResult.Value);
        }

        [Fact]
        public async Task GetCompany_ReturnsOk_WithCompanyDto()
        {
            var id = Guid.NewGuid();
            var companyDto = new CompanyDto { Id = id, Name = "Company 123" };

            _mediatorMock.Setup(m => m.Send(It.Is<GetCompanyQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companyDto);

            var result = await _controller.GetCompany(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(companyDto, okResult.Value);
        }

        [Fact]
        public async Task GetAllCompanies_ReturnsOk_WithCompanyDtoList()
        {
            var companies = new List<CompanyDto>
        {
            new CompanyDto { Id = Guid.NewGuid(), Name = "Company 1" },
            new CompanyDto { Id = Guid.NewGuid(), Name = "Company 2" }
        };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCompaniesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companies);

            var result = await _controller.GetAllCompanies();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(companies, okResult.Value);
        }

        [Fact]
        public async Task DeleteCompany_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteCompanyCommand>(c => c.Id == id), It.IsAny<CancellationToken>()));

            var result = await _controller.DeleteCompany(id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}