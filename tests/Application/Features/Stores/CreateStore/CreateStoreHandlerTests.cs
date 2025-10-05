using System.Net;
using Application.Common;
using Application.Dtos;
using Application.Features.Stores.CreateStore;
using Application.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace tests.Application.Features.Stores.CreateStore
{
    public class CreateStoreHandlerTests
    {
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateStoreHandler _handler;

        public CreateStoreHandlerTests()
        {
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new StoreMapping())).CreateMapper();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new CreateStoreHandler(_mockStoreRepository.Object, _mockCompanyRepository.Object, _mapper, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ValidStoreData_ShouldCreateStoreSuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = "Test Store",
                Address = "123 Test Street",
                City = "Test City",
                Country = "Test Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var company = new Company { Id = companyId, Name = "Test Company" };

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .ReturnsAsync((Store)null);

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
            
            _mockStoreRepository.Verify(x => x.GetStoreByNameAsync(request.Name), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockStoreRepository.Verify(x => x.AddAsync(It.Is<Store>(s => 
                s.Name == request.Name && 
                s.Address == request.Address && 
                s.City == request.City && 
                s.Country == request.Country && 
                s.CompanyId == companyId)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DuplicateStoreName_ShouldThrowCustomExceptionWithUnprocessableEntityStatus()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = "Duplicate Store",
                Address = "123 Test Street",
                City = "Test City",
                Country = "Test Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var existingStore = new Store { Name = "Duplicate Store" };

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .ReturnsAsync(existingStore);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Store with the same name already exists.", exception.Message);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            _mockStoreRepository.Verify(x => x.GetStoreByNameAsync(request.Name), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockStoreRepository.Verify(x => x.AddAsync(It.IsAny<Store>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_NonExistentCompany_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = "Test Store",
                Address = "123 Test Street",
                City = "Test City",
                Country = "Test Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .ReturnsAsync((Store)null);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            _mockStoreRepository.Verify(x => x.GetStoreByNameAsync(request.Name), Times.Once);
            _mockCompanyRepository.Verify(x => x.GetByIdAsync(companyId), Times.Once);
            _mockStoreRepository.Verify(x => x.AddAsync(It.IsAny<Store>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCallRepositoryWithCorrectStoreObject()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = "Correct Store",
                Address = "456 Correct Street",
                City = "Correct City",
                Country = "Correct Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var company = new Company { Id = companyId, Name = "Test Company" };
            Store capturedStore = null;

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .ReturnsAsync((Store)null);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(company);

            _mockStoreRepository
                .Setup(x => x.AddAsync(It.IsAny<Store>()))
                .Callback<Store>(store => capturedStore = store);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(capturedStore);
            Assert.Equal(request.Name, capturedStore.Name);
            Assert.Equal(request.Address, capturedStore.Address);
            Assert.Equal(request.City, capturedStore.City);
            Assert.Equal(request.Country, capturedStore.Country);
            Assert.Equal(companyId, capturedStore.CompanyId);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnMappedStoreDto()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = "Mapping Test Store",
                Address = "789 Mapping Street",
                City = "Mapping City",
                Country = "Mapping Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var company = new Company { Id = companyId, Name = "Test Company" };

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .ReturnsAsync((Store)null);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(company);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<StoreDto>(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Address, result.Address);
            Assert.Equal(request.City, result.City);
            Assert.Equal(request.Country, result.Country);
        }

        [Theory]
        [InlineData("Store A", "123 Main St", "New York", "USA")]
        [InlineData("Store B", "456 Oak Ave", "London", "UK")]
        [InlineData("Store C", "789 Pine Rd", "Toronto", "Canada")]
        public async Task Handle_DifferentStoreData_ShouldCreateStoreCorrectly(string name, string address, string city, string country)
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = name,
                Address = address,
                City = city,
                Country = country,
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var company = new Company { Id = companyId, Name = "Test Company" };

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(name))
                .ReturnsAsync((Store)null);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .ReturnsAsync(company);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Equal(address, result.Address);
            Assert.Equal(city, result.City);
            Assert.Equal(country, result.Country);
        }

        [Fact]
        public async Task Handle_EmptyCompanyId_ShouldThrowCustomExceptionWithNotFoundStatus()
        {
            // Arrange
            var request = new CreateStoreCommand
            {
                Name = "Test Store",
                Address = "123 Test Street",
                City = "Test City",
                Country = "Test Country",
                CompanyId = Guid.Empty
            };
            var cancellationToken = CancellationToken.None;

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .ReturnsAsync((Store)null);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(Guid.Empty))
                .ReturnsAsync((Company)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(() => _handler.Handle(request, cancellationToken));
            
            Assert.Equal("Company not found.", exception.Message);
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCheckStoreNameBeforeCheckingCompany()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = "Sequence Test Store",
                Address = "123 Sequence Street",
                City = "Sequence City",
                Country = "Sequence Country",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var company = new Company { Id = companyId, Name = "Test Company" };
            var callOrder = new List<string>();

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .Callback(() => callOrder.Add("CheckStoreName"))
                .ReturnsAsync((Store)null);

            _mockCompanyRepository
                .Setup(x => x.GetByIdAsync(companyId))
                .Callback(() => callOrder.Add("CheckCompany"))
                .ReturnsAsync(company);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.True(callOrder.Count >= 2);
            Assert.Equal("CheckStoreName", callOrder[0]);
            Assert.Equal("CheckCompany", callOrder[1]);
        }

        [Fact]
        public async Task Handle_StoreWithSpecialCharacters_ShouldCreateSuccessfully()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var request = new CreateStoreCommand
            {
                Name = "Store with Special Characters !@#$%",
                Address = "123 Special Street & Ave",
                City = "São Paulo",
                Country = "Côte d'Ivoire",
                CompanyId = companyId
            };
            var cancellationToken = CancellationToken.None;
            var company = new Company { Id = companyId, Name = "Test Company" };

            _mockStoreRepository
                .Setup(x => x.GetStoreByNameAsync(request.Name))
                .ReturnsAsync((Store)null);

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
        }
    }
}