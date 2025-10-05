using System.Net;
using Application.Common;
using Application.Dtos;
using Application.Features.Companies.CreateCompany;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace tests.Application.Features.Companies.CreateCompany
{
    public class CreateCompanyHandlerTests
    {
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateCompanyHandler _sut; // System Under Test

        public CreateCompanyHandlerTests()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _sut = new CreateCompanyHandler(
                _mockCompanyRepository.Object, 
                _mockMapper.Object, 
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameIsUnique_ShouldCreateCompanySuccessfully()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Unique Company Name" };
            var expectedCompanyDto = new CompanyDto { Name = "Unique Company Name" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync((Company)null!);

            _mockMapper
                .Setup(mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()))
                .Returns(expectedCompanyDto);

            // Act
            var result = await _sut.Handle(command, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCompanyDto.Name, result.Name);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameIsUnique_ShouldCheckIfCompanyExistsByName()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Test Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync((Company)null!);

            _mockMapper
                .Setup(mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()))
                .Returns(new CompanyDto());

            // Act
            await _sut.Handle(command, cancellationToken);

            // Assert
            _mockCompanyRepository.Verify(
                repo => repo.GetCompanyByNameAsync(command.Name), 
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameIsUnique_ShouldAddCompanyToRepository()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "New Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync((Company)null!);

            _mockMapper
                .Setup(mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()))
                .Returns(new CompanyDto());

            // Act
            await _sut.Handle(command, cancellationToken);

            // Assert
            _mockCompanyRepository.Verify(
                repo => repo.AddAsync(It.Is<Company>(c => c.Name == command.Name)), 
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameIsUnique_ShouldSaveChangesToDatabase()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Save Test Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync((Company)null!);

            _mockMapper
                .Setup(mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()))
                .Returns(new CompanyDto());

            // Act
            await _sut.Handle(command, cancellationToken);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameIsUnique_ShouldMapCompanyEntityToDto()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Mapping Test Company" };
            var cancellationToken = CancellationToken.None;
            var expectedDto = new CompanyDto { Name = "Mapping Test Company" };

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync((Company)null!);

            _mockMapper
                .Setup(mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()))
                .Returns(expectedDto);

            // Act
            await _sut.Handle(command, cancellationToken);

            // Assert
            _mockMapper.Verify(
                mapper => mapper.Map<CompanyDto>(It.Is<Company>(c => c.Name == command.Name)), 
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameAlreadyExists_ShouldThrowCustomExceptionWithCorrectMessage()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Existing Company" };
            var existingCompany = new Company { Name = "Existing Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync(existingCompany);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(
                () => _sut.Handle(command, cancellationToken));

            Assert.Equal("Company with the same name already exists.", exception.Message);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameAlreadyExists_ShouldThrowCustomExceptionWithUnprocessableEntityStatus()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Duplicate Company" };
            var existingCompany = new Company { Name = "Duplicate Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync(existingCompany);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomException>(
                () => _sut.Handle(command, cancellationToken));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameAlreadyExists_ShouldNotAddCompanyToRepository()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Existing Company" };
            var existingCompany = new Company { Name = "Existing Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync(existingCompany);

            // Act & Assert
            await Assert.ThrowsAsync<CustomException>(
                () => _sut.Handle(command, cancellationToken));

            _mockCompanyRepository.Verify(
                repo => repo.AddAsync(It.IsAny<Company>()), 
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameAlreadyExists_ShouldNotSaveChangesToDatabase()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Existing Company" };
            var existingCompany = new Company { Name = "Existing Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync(existingCompany);

            // Act & Assert
            await Assert.ThrowsAsync<CustomException>(
                () => _sut.Handle(command, cancellationToken));

            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCompanyNameAlreadyExists_ShouldNotMapCompanyToDto()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Existing Company" };
            var existingCompany = new Company { Name = "Existing Company" };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync(existingCompany);

            // Act & Assert
            await Assert.ThrowsAsync<CustomException>(
                () => _sut.Handle(command, cancellationToken));

            _mockMapper.Verify(
                mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()), 
                Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("A")]
        [InlineData("Company with Special Characters !@#$%^&*()")]
        public async Task Handle_WhenGivenVariousValidCompanyNames_ShouldCreateCompanySuccessfully(string companyName)
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = companyName };
            var expectedDto = new CompanyDto { Name = companyName };
            var cancellationToken = CancellationToken.None;

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(companyName))
                .ReturnsAsync((Company)null!);

            _mockMapper
                .Setup(mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()))
                .Returns(expectedDto);

            // Act
            var result = await _sut.Handle(command, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(companyName, result.Name);
        }

        [Fact]
        public async Task Handle_WhenOperationCompletes_ShouldExecuteOperationsInCorrectOrder()
        {
            // Arrange
            var command = new CreateCompanyCommand { Name = "Order Test Company" };
            var cancellationToken = CancellationToken.None;
            var callOrder = new List<string>();

            _mockCompanyRepository
                .Setup(repo => repo.GetCompanyByNameAsync(command.Name))
                .ReturnsAsync((Company)null!)
                .Callback(() => callOrder.Add("GetCompanyByNameAsync"));

            _mockCompanyRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Company>()))
                .Callback(() => callOrder.Add("AddAsync"));

            _mockUnitOfWork
                .Setup(uow => uow.SaveChangesAsync())
                .Callback(() => callOrder.Add("SaveChangesAsync"));

            _mockMapper
                .Setup(mapper => mapper.Map<CompanyDto>(It.IsAny<Company>()))
                .Returns(new CompanyDto())
                .Callback(() => callOrder.Add("Map"));

            // Act
            await _sut.Handle(command, cancellationToken);

            // Assert
            Assert.Equal(new[] { "GetCompanyByNameAsync", "AddAsync", "SaveChangesAsync", "Map" }, callOrder);
        }
    }
}
