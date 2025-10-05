using System.Net;
using Application.Common;
using Application.Dtos;
using Application.Features.Stores.GetStore;
using Application.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace tests.Application.Features.Stores.GetStore
{
    public class GetStoreHandlerTests
    {
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly IMapper _mapper;
        private readonly GetStoreHandler _handler;

        public GetStoreHandlerTests()
        {
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mapper = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile(new StoreMapping());
                cfg.AddProfile(new CompanyMapping());
            }).CreateMapper();
            _handler = new GetStoreHandler(_mockStoreRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ExistingStoreId_ShouldReturnDetailedStoreDtoSuccessfully()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var request = new GetStoreQuery { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store
            {
                Id = storeId,
                Name = "Test Store",
                Address = "123 Test Street",
                City = "Test City",
                Country = "Test Country",
                CompanyId = companyId,
                Company = new Company { Id = companyId, Name = "Test Company" }
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingStore.Id, result.Id);
            Assert.Equal(existingStore.Name, result.Name);
            Assert.Equal(existingStore.Address, result.Address);
            Assert.Equal(existingStore.City, result.City);
            Assert.Equal(existingStore.Country, result.Country);
            Assert.NotNull(result.Company);
            Assert.Equal(existingStore.Company.Id, result.Company.Id);
            Assert.Equal(existingStore.Company.Name, result.Company.Name);
            
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentStoreId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var request = new GetStoreQuery { Id = storeId };
            var cancellationToken = CancellationToken.None;

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync((Store)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Store not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidStoreId_ShouldReturnDetailedStoreDtoType()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var request = new GetStoreQuery { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store
            {
                Id = storeId,
                Name = "Type Test Store",
                Address = "123 Type Street",
                City = "Type City",
                Country = "Type Country",
                Company = new Company { Name = "Type Company" }
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DetailedStoreDto>(result);
            Assert.IsAssignableFrom<StoreDto>(result);
        }
        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000")]
        [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        public async Task Handle_DifferentValidStoreIds_ShouldReturnStoreSuccessfully(string guidString)
        {
            // Arrange
            var storeId = Guid.Parse(guidString);
            var request = new GetStoreQuery { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store
            {
                Id = storeId,
                Name = $"Store {storeId}",
                Address = "Test Address",
                City = "Test City",
                Country = "Test Country",
                Company = new Company { Name = "Test Company" }
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.Id);
            Assert.Equal($"Store {storeId}", result.Name);
            _mockStoreRepository.Verify(x => x.GetByIdAsync(storeId), Times.Once);
        }


        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnExactMappedResult()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            var request = new GetStoreQuery { Id = storeId };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store
            {
                Id = storeId,
                Name = "Exact Mapping Test Store",
                Address = "456 Exact Street",
                City = "Exact City",
                Country = "Exact Country",
                CompanyId = companyId,
                Company = new Company { Id = companyId, Name = "Exact Company" }
            };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(existingStore);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingStore.Id, result.Id);
            Assert.Equal(existingStore.Name, result.Name);
            Assert.Equal(existingStore.Address, result.Address);
            Assert.Equal(existingStore.City, result.City);
            Assert.Equal(existingStore.Country, result.Country);
            Assert.NotNull(result.Company);
            Assert.Equal(existingStore.Company.Id, result.Company.Id);
            Assert.Equal(existingStore.Company.Name, result.Company.Name);
        }

    }
}