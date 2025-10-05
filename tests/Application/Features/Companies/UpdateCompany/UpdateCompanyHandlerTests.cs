using System.Net;
using Application.Common;
using Application.Dtos;
using Application.Features.Companies.UpdateCompany;
using Application.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace tests.Application.Features.Companies.UpdateCompany
{
    public class UpdateCompanyHandlerTests
    {
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateCompanyHandler _handler;

        public UpdateCompanyHandlerTests()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new CompanyMapping())).CreateMapper();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new UpdateCompanyHandler(_mockCompanyRepository.Object, _mapper, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ExistingCompanyWithValidNewName_ShouldUpdateCompanySuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new UpdateCompanyCommand { Id = companyId, Name = "Updated Company Name" };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Id = companyId, Name = "Original Company Name" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.GetCompanyByNameAsync(request.Name))
                .ReturnsAsync((Company)null);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(companyId, result.Id);
            Assert.Equal(request.Name, existingCompany.Name);
            
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetCompanyByNameAsync(request.Name), Times.Once);
            _mockCompanyRepository.Verify(x => x.UpdateAsync(existingCompany), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentCompanyId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new UpdateCompanyCommand { Id = companyId, Name = "New Company Name" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetCompanyByNameAsync(It.IsAny<string>()), Times.Never);
            _mockCompanyRepository.Verify(x => x.UpdateAsync(It.IsAny<Company>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_DuplicateCompanyName_ShouldThrowCustomExceptionWithUnprocessableEntityStatus()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var anotherCompanyId = Guid.NewGuid();
            var request = new UpdateCompanyCommand { Id = companyId, Name = "Duplicate Name" };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Id = companyId, Name = "Original Name" };
            var companyWithSameName = new Company { Id = anotherCompanyId, Name = "Duplicate Name" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.GetCompanyByNameAsync(request.Name))
                .ReturnsAsync(companyWithSameName);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Another company with the same name already exists.", exception.Message);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetCompanyByNameAsync(request.Name), Times.Once);
            _mockCompanyRepository.Verify(x => x.UpdateAsync(It.IsAny<Company>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateCompanyWithSameName_ShouldUpdateSuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new UpdateCompanyCommand { Id = companyId, Name = "Same Company Name" };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Id = companyId, Name = "Original Name" };
            var companyWithSameName = new Company { Id = companyId, Name = "Same Company Name" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.GetCompanyByNameAsync(request.Name))
                .ReturnsAsync(companyWithSameName);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(companyId, result.Id);
            Assert.Equal(request.Name, existingCompany.Name);
            
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetCompanyByNameAsync(request.Name), Times.Once);
            _mockCompanyRepository.Verify(x => x.UpdateAsync(existingCompany), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidUpdate_ShouldReturnMappedCompanyDto()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new UpdateCompanyCommand { Id = companyId, Name = "Mapped Company Name" };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Id = companyId, Name = "Original Name" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.GetCompanyByNameAsync(request.Name))
                .ReturnsAsync((Company)null);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingCompany.Id, result.Id);
            Assert.Equal(existingCompany.Name, result.Name);
            Assert.Equal(request.Name, result.Name);
        }

        [Theory]
        [InlineData("Simple Updated Name")]
        [InlineData("Company with Special Characters !@#$%")]
        [InlineData("   Company with Spaces   ")]
        [InlineData("Company123WithNumbers456")]
        public async Task Handle_DifferentCompanyNames_ShouldUpdateCorrectly(string newCompanyName)
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new UpdateCompanyCommand { Id = companyId, Name = newCompanyName };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Id = companyId, Name = "Original Name" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.GetCompanyByNameAsync(newCompanyName))
                .ReturnsAsync((Company)null);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newCompanyName, result.Name);
            Assert.Equal(newCompanyName, existingCompany.Name);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var companyId = Guid.Empty;
            var request = new UpdateCompanyCommand { Id = companyId, Name = "New Name" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task Handle_ValidUpdate_ShouldModifyExistingCompanyNameProperty()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var originalName = "Original Company Name";
            var newName = "Updated Company Name";
            var request = new UpdateCompanyCommand { Id = companyId, Name = newName };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Id = companyId, Name = originalName };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.GetCompanyByNameAsync(newName))
                .ReturnsAsync((Company)null);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.Equal(newName, existingCompany.Name);
            Assert.NotEqual(originalName, existingCompany.Name);
        }
    }
}