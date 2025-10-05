using Application.Dtos;
using Application.Features.Stores.CreateStore;
using Application.Features.Stores.DeleteStore;
using Application.Features.Stores.GetStore;
using Application.Features.Stores.GetStores;
using Application.Features.Stores.UpdateStore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StoreApi.Controllers;

namespace tests.Controllers
{
    public class StoreControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly StoreController _controller;

        public StoreControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new StoreController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateStore_ReturnsCreatedAtAction_WithStoreDto()
        {
            var command = new CreateStoreCommand
            {
                Name = "Test Store",
                Address = "Street 1",
                City = "City",
                Country = "Country",
                CompanyId = Guid.NewGuid()
            };
            var storeDto = new StoreDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Store",
                Address = "Street 1",
                City = "City",
                Country = "Country"
            };

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(storeDto);

            var result = await _controller.CreateStore(command);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(storeDto, createdResult.Value);
        }

        [Fact]
        public async Task UpdateStore_ReturnsOk_WithStoreDto()
        {
            var id = Guid.NewGuid();
            var command = new UpdateStoreCommand
            {
                Name = "Updated Store",
                Address = "Street 2",
                City = "New City",
                Country = "Country",
                CompanyId = Guid.NewGuid()
            };
            var storeDto = new StoreDto
            {
                Id = id,
                Name = "Updated Store",
                Address = "Street 2",
                City = "New City",
                Country = "Country"
            };

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateStoreCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(storeDto);

            var result = await _controller.UpdateStore(id, command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(storeDto, okResult.Value);
        }

        [Fact]
        public async Task GetStore_ReturnsOk_WithDetailedStoreDto()
        {
            var id = Guid.NewGuid();
            var detailedStoreDto = new DetailedStoreDto
            {
                Id = id,
                Name = "Detailed Store",
                Address = "Street 3",
                City = "City",
                Country = "Country",
                Company = new CompanyDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Company"
                }
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetStoreQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(detailedStoreDto);

            var result = await _controller.GetStore(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(detailedStoreDto, okResult.Value);
        }

        [Fact]
        public async Task GetAllStores_ReturnsOk_WithStoreDtoList()
        {
            var stores = new List<StoreDto>
        {
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 1", Address = "Street 1", City = "City", Country = "Country" },
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 2", Address = "Street 2", City = "City", Country = "Country" }
        };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStoresQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stores);

            var result = await _controller.GetAllStores();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(stores, okResult.Value);
        }

        [Fact]
        public async Task DeleteStore_ReturnsNoContent()
        {
            var id = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.Is<DeleteStoreCommand>(c => c.Id == id), It.IsAny<CancellationToken>()));

            var result = await _controller.DeleteStore(id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}