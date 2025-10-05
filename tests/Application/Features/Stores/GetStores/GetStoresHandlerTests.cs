using Application.Dtos;
using Application.Features.Stores.GetStores;
using Application.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace tests.Application.Features.Stores.GetStores
{
    public class GetStoresHandlerTests
    {
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly IMapper _mapper;
        private readonly GetStoresHandler _handler;

        public GetStoresHandlerTests()
        {
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new StoreMapping())).CreateMapper();
            _handler = new GetStoresHandler(_mockStoreRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WithExistingStores_ShouldReturnAllStoresSortedByName()
        {
            // Arrange
            var request = new GetStoresQuery();
            var cancellationToken = CancellationToken.None;
            var stores = new List<Store>
            {
                new Store 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Zebra Store", 
                    Address = "123 Zebra St", 
                    City = "Zebra City", 
                    Country = "Zebra Country" 
                },
                new Store 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Alpha Store", 
                    Address = "456 Alpha Ave", 
                    City = "Alpha City", 
                    Country = "Alpha Country" 
                },
                new Store 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Beta Store", 
                    Address = "789 Beta Blvd", 
                    City = "Beta City", 
                    Country = "Beta Country" 
                }
            };

            _mockStoreRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(stores);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            
            // Verify sorting by name
            Assert.Equal("Alpha Store", resultList[0].Name);
            Assert.Equal("Beta Store", resultList[1].Name);
            Assert.Equal("Zebra Store", resultList[2].Name);
            
            // Verify all properties are mapped correctly
            Assert.Equal("456 Alpha Ave", resultList[0].Address);
            Assert.Equal("Alpha City", resultList[0].City);
            Assert.Equal("Alpha Country", resultList[0].Country);
            
            _mockStoreRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyStoresList_ShouldReturnEmptyCollection()
        {
            // Arrange
            var request = new GetStoresQuery();
            var cancellationToken = CancellationToken.None;
            var stores = new List<Store>();

            _mockStoreRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(stores);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockStoreRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithSingleStore_ShouldReturnSingleStoreDto()
        {
            // Arrange
            var request = new GetStoresQuery();
            var cancellationToken = CancellationToken.None;
            var storeId = Guid.NewGuid();
            var stores = new List<Store>
            {
                new Store 
                { 
                    Id = storeId, 
                    Name = "Single Store", 
                    Address = "123 Single St", 
                    City = "Single City", 
                    Country = "Single Country" 
                }
            };

            _mockStoreRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(stores);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(storeId, resultList[0].Id);
            Assert.Equal("Single Store", resultList[0].Name);
            Assert.Equal("123 Single St", resultList[0].Address);
            Assert.Equal("Single City", resultList[0].City);
            Assert.Equal("Single Country", resultList[0].Country);
            _mockStoreRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

    }
}