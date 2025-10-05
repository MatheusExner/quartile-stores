using System.Net;
using Application.Common;
using Application.Dtos;
using Application.Features.Companies.GetCompany;
using Application.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace tests.Application.Features.Companies.GetCompany
{
    public class GetCompanyHandlerTests
    {
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly IMapper _mapper;
        private readonly GetCompanyHandler _handler;

        public GetCompanyHandlerTests()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new CompanyMapping())).CreateMapper();
            _handler = new GetCompanyHandler(_mockCompanyRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ExistingCompanyId_ShouldReturnCompanyDtoSuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new GetCompanyQuery { Id = companyId };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Name = "Test Company" };
            var expectedCompanyDto = new CompanyDto { Name = "Test Company" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCompanyDto.Name, result.Name);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentCompanyId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new GetCompanyQuery { Id = companyId };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));

            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCompanyId_ShouldCallMapperWithCorrectCompanyObject()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new GetCompanyQuery { Id = companyId };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Name = "Mapping Test Company" };
            var expectedDto = new CompanyDto { Name = "Mapping Test Company", Id = existingCompany.Id };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingCompany.Id, result.Id);
            Assert.Equal(existingCompany.Name, result.Name);
        }


        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        public async Task Handle_DifferentValidCompanyIds_ShouldReturnCompanySuccessfully(string guidString)
        {
            // Arrange
            var companyId = Guid.Parse(guidString);
            var request = new GetCompanyQuery { Id = companyId };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Name = $"Company {companyId}" };
            var expectedDto = new CompanyDto { Name = $"Company {companyId}" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Name, result.Name);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var companyId = Guid.Empty;
            var request = new GetCompanyQuery { Id = companyId };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));

            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
        }

    }

}