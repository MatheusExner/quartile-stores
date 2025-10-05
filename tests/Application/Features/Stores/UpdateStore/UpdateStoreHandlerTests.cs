using System.Net;
using Application.Common;
using Application.Dtos;
using Application.Features.Stores.UpdateStore;
using Application.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace tests.Application.Features.Stores.UpdateStore
{
    public class UpdateStoreHandlerTests
    {
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly UpdateStoreHandler _handler;

        public UpdateStoreHandlerTests()
        {
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new StoreMapping())).CreateMapper();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new UpdateStoreHandler(_mockStoreRepository.Object, _mockCompanyRepository.Object, _mapper, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ExistingStoreAndCompany_ShouldUpdateStoreSuccessfully()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var request = new UpdateStoreCommand
            {
                Id = storeId,
                Name = "Updated Store Name",
                Address = "Updated Address",
                City = "Updated City",
                Country = "Updated Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store
            {
                Id = storeId,
                Name = "Original Store Name",
                Address = "Original Address",
                City = "Original City",
                Country = "Original Country",
                CompanyId = Guid.NewGuid()
            };
            var company = new Company { Id = companyId, Name = "Test Company" };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(company);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Address, result.Address);
            Assert.Equal(request.City, result.City);
            Assert.Equal(request.Country, result.Country);
            
            // Verify the store was updated with new values
            Assert.Equal(request.Name, existingStore.Name);
            Assert.Equal(request.Address, existingStore.Address);
            Assert.Equal(request.City, existingStore.City);
            Assert.Equal(request.Country, existingStore.Country);
            Assert.Equal(companyId, existingStore.CompanyId);
            
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockStoreRepository.Verify(x => x.UpdateAsync(existingStore), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentStore_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var request = new UpdateStoreCommand
            {
                Id = storeId,
                Name = "Updated Store Name",
                Address = "Updated Address",
                City = "Updated City",
                Country = "Updated Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync((Store)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Store not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockStoreRepository.Verify(x => x.UpdateAsync(It.IsAny<Store>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_NonExistentCompany_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var request = new UpdateStoreCommand
            {
                Id = storeId,
                Name = "Updated Store Name",
                Address = "Updated Address",
                City = "Updated City",
                Country = "Updated Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store
            {
                Id = storeId,
                Name = "Original Store Name",
                Address = "Original Address",
                City = "Original City",
                Country = "Original Country"
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockStoreRepository.Verify(x => x.UpdateAsync(It.IsAny<Store>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidUpdate_ShouldReturnMappedStoreDto()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var request = new UpdateStoreCommand
            {
                Id = storeId,
                Name = "Mapping Test Store",
                Address = "Mapping Test Address",
                City = "Mapping Test City",
                Country = "Mapping Test Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store { Id = storeId, Name = "Original Name" };
            var company = new Company { Id = companyId, Name = "Test Company" };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(company);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<StoreDto>(result);
            Assert.Equal(existingStore.Id, result.Id);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Address, result.Address);
            Assert.Equal(request.City, result.City);
            Assert.Equal(request.Country, result.Country);
        }



        [Fact]
        public async Task Handle_EmptyStoreId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var storeId = Guid.Empty;
            var companyId = Guid.NewGuid();
            var request = new UpdateStoreCommand
            {
                Id = storeId,
                Name = "Test Store",
                Address = "Test Address",
                City = "Test City",
                Country = "Test Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync((Store)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Store not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

    }
}