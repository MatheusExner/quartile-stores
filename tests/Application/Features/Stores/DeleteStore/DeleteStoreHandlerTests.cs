using System.Net;
using Application.Common;
using Application.Features.Stores.DeleteStore;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace tests.Application.Features.Stores.DeleteStore
{
    public class DeleteStoreHandlerTests
    {
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteStoreHandler _handler;

        public DeleteStoreHandlerTests()
        {
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new DeleteStoreHandler(_mockStoreRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ExistingStoreId_ShouldDeleteStoreSuccessfully()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var request = new DeleteStoreCommand { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store 
            { 
                Id = storeId,
                Name = "Test Store",
                Address = "123 Test Street",
                City = "Test City",
                Country = "Test Country"
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockStoreRepository.Verify(x => x.DeleteAsync(existingStore), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentStoreId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var request = new DeleteStoreCommand { Id = storeId };
            var cancellationToken = CancellationToken.None;

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync((Store)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Store not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockStoreRepository.Verify(x => x.DeleteAsync(It.IsAny<Store>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidStoreId_ShouldCallRepositoryWithCorrectStoreObject()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var request = new DeleteStoreCommand { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store 
            { 
                Id = storeId,
                Name = "Store To Delete",
                Address = "456 Delete Street",
                City = "Delete City",
                Country = "Delete Country"
            };
            Store deletedStore = null;

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            _mockStoreRepository
                .Setup(x => x.DeleteAsync(It.IsAny<Store>()))
                .Callback<Store>(store => deletedStore = store);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(deletedStore);
            Assert.Equal(existingStore, deletedStore);
            Assert.Equal(existingStore.Id, deletedStore.Id);
            Assert.Equal(existingStore.Name, deletedStore.Name);
            Assert.Equal(existingStore.Address, deletedStore.Address);
            Assert.Equal(existingStore.City, deletedStore.City);
            Assert.Equal(existingStore.Country, deletedStore.Country);
        }


        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        public async Task Handle_DifferentValidStoreIds_ShouldDeleteSuccessfully(string guidString)
        {
            // Arrange
            var storeId = Guid.Parse(guidString);
            var request = new DeleteStoreCommand { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store 
            { 
                Id = storeId, 
                Name = $"Store {storeId}",
                Address = "Test Address",
                City = "Test City",
                Country = "Test Country"
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockStoreRepository.Verify(x => x.DeleteAsync(existingStore), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyGuidId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var storeId = Guid.Empty;
            var request = new DeleteStoreCommand { Id = storeId };
            var cancellationToken = CancellationToken.None;

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync((Store)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Store not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockStoreRepository.Verify(x => x.DeleteAsync(It.IsAny<Store>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_StoreWithCompanyAssociation_ShouldDeleteSuccessfully()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var request = new DeleteStoreCommand { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store 
            { 
                Id = storeId,
                Name = "Store with Company",
                Address = "123 Company Street",
                City = "Company City",
                Country = "Company Country",
                CompanyId = companyId
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockStoreRepository.Verify(x => x.DeleteAsync(existingStore), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_StoreWithSpecialCharactersInName_ShouldDeleteSuccessfully()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var request = new DeleteStoreCommand { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store 
            { 
                Id = storeId,
                Name = "Store with Special Characters !@#$%",
                Address = "123 Special Street & Ave",
                City = "São Paulo",
                Country = "Côte d'Ivoire"
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
            _mockStoreRepository.Verify(x => x.DeleteAsync(existingStore), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

    }
}