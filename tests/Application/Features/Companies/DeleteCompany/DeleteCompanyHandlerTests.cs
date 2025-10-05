using System.Net;
using Application.Common;
using Application.Features.Companies.DeleteCompany;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace tests.Application.Features.Companies.DeleteCompany
{
    public class DeleteCompanyHandlerTests
    {
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteCompanyHandler _handler;

        public DeleteCompanyHandlerTests()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new DeleteCompanyHandler(_mockCompanyRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ExistingCompanyId_ShouldDeleteCompanySuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new DeleteCompanyCommand { Id = companyId };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Name = "Test Company" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockCompanyRepository.Verify(x => x.DeleteAsync(existingCompany), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentCompanyId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new DeleteCompanyCommand { Id = companyId };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockCompanyRepository.Verify(x => x.DeleteAsync(It.IsAny<Company>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidCompanyId_ShouldCallRepositoryWithCorrectCompanyObject()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new DeleteCompanyCommand { Id = companyId };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Name = "Company To Delete" };
            Company deletedCompany = null;

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.DeleteAsync(It.IsAny<Company>()))
                .Callback<Company>(company => deletedCompany = company);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(deletedCompany);
            Assert.Equal(existingCompany, deletedCompany);
            Assert.Equal(existingCompany.Name, deletedCompany.Name);
        }

        [Fact]
        public async Task Handle_ValidCompanyId_ShouldCallUnitOfWorkSaveChangesAfterDeletion()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new DeleteCompanyCommand { Id = companyId };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Name = "Test Company" };
            var callOrder = new List<string>();

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            _mockCompanyRepository
                .Setup(x => x.DeleteAsync(It.IsAny<Company>()))
                .Callback(() => callOrder.Add("Delete"));

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .Callback(() => callOrder.Add("Save"));

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.Equal(2, callOrder.Count);
            Assert.Equal("Delete", callOrder[0]);
            Assert.Equal("Save", callOrder[1]);
        }

        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        public async Task Handle_DifferentValidCompanyIds_ShouldDeleteSuccessfully(string guidString)
        {
            // Arrange
            var companyId = Guid.Parse(guidString);
            var request = new DeleteCompanyCommand { Id = companyId };
            var cancellationToken = CancellationToken.None;
            var existingCompany = new Company { Name = $"Company {companyId}" };

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(existingCompany);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockCompanyRepository.Verify(x => x.DeleteAsync(existingCompany), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var companyId = Guid.Empty;
            var request = new DeleteCompanyCommand { Id = companyId };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}