using Application.Dtos;
using Application.Features.Companies.GetCompanies;
using Application.Mapping;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace tests.Application.Features.Companies.GetCompanies
{
    public class GetCompaniesHandlerTests
    {
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly IMapper _mapper;
        private readonly GetCompaniesHandler _handler;

        public GetCompaniesHandlerTests()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new CompanyMapping())).CreateMapper();
            _handler = new GetCompaniesHandler(_mockCompanyRepository.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WithExistingCompanies_ShouldReturnAllCompaniesSortedByName()
        {
            // Arrange
            var request = new GetCompaniesQuery();
            var cancellationToken = CancellationToken.None;
            var companies = new List<Company>
            {
                new Company { Id = Guid.NewGuid(), Name = "Zebra Company" },
                new Company { Id = Guid.NewGuid(), Name = "Alpha Company" },
                new Company { Id = Guid.NewGuid(), Name = "Beta Company" }
            };

            _mockCompanyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(companies);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            
            // Verify sorting by name
            Assert.Equal("Alpha Company", resultList[0].Name);
            Assert.Equal("Beta Company", resultList[1].Name);
            Assert.Equal("Zebra Company", resultList[2].Name);
            
            _mockCompanyRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyCompaniesList_ShouldReturnEmptyCollection()
        {
            // Arrange
            var request = new GetCompaniesQuery();
            var cancellationToken = CancellationToken.None;
            var companies = new List<Company>();

            _mockCompanyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(companies);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCompanyRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithSingleCompany_ShouldReturnSingleCompanyDto()
        {
            // Arrange
            var request = new GetCompaniesQuery();
            var cancellationToken = CancellationToken.None;
            var companyId = Guid.NewGuid();
            var companies = new List<Company>
            {
                new Company { Id = companyId, Name = "Single Company" }
            };

            _mockCompanyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(companies);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(companyId, resultList[0].Id);
            Assert.Equal("Single Company", resultList[0].Name);
            _mockCompanyRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithCompaniesHavingSameName_ShouldReturnAllCompaniesInCorrectOrder()
        {
            // Arrange
            var request = new GetCompaniesQuery();
            var cancellationToken = CancellationToken.None;
            var companies = new List<Company>
            {
                new Company { Id = Guid.NewGuid(), Name = "Same Name" },
                new Company { Id = Guid.NewGuid(), Name = "Same Name" },
                new Company { Id = Guid.NewGuid(), Name = "Different Name" }
            };

            _mockCompanyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(companies);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            
            // First should be "Different Name", then the two "Same Name" entries
            Assert.Equal("Different Name", resultList[0].Name);
            Assert.Equal("Same Name", resultList[1].Name);
            Assert.Equal("Same Name", resultList[2].Name);
            
            _mockCompanyRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithCompaniesHavingStores_ShouldReturnOnlyCompanyDtoProperties()
        {
            // Arrange
            var request = new GetCompaniesQuery();
            var cancellationToken = CancellationToken.None;
            var companies = new List<Company>
            {
                new Company 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Company with Stores",
                    Stores = new List<Store> { new Store(), new Store() }
                }
            };

            _mockCompanyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(companies);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(companies[0].Id, resultList[0].Id);
            Assert.Equal(companies[0].Name, resultList[0].Name);
            // CompanyDto should not expose Stores property
            _mockCompanyRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce()
        {
            // Arrange
            var request = new GetCompaniesQuery();
            var cancellationToken = CancellationToken.None;
            var companies = new List<Company>
            {
                new Company { Id = Guid.NewGuid(), Name = "Test Company" }
            };

            _mockCompanyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(companies);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mockCompanyRepository.Verify(x => x.GetAllAsync(), Times.Once);
            _mockCompanyRepository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("A Company", "B Company", "C Company")]
        [InlineData("zebra", "alpha", "beta")]
        [InlineData("123 Company", "456 Company", "789 Company")]
        public async Task Handle_WithDifferentNameFormats_ShouldSortCorrectly(string name1, string name2, string name3)
        {
            // Arrange
            var request = new GetCompaniesQuery();
            var cancellationToken = CancellationToken.None;
            var companies = new List<Company>
            {
                new Company { Id = Guid.NewGuid(), Name = name3 }, // Last alphabetically
                new Company { Id = Guid.NewGuid(), Name = name1 }, // First alphabetically
                new Company { Id = Guid.NewGuid(), Name = name2 }  // Middle alphabetically
            };

            _mockCompanyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(companies);

            // Act
            var result = await _handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            
            var sortedNames = new[] { name1, name2, name3 }.OrderBy(n => n).ToArray();
            Assert.Equal(sortedNames[0], resultList[0].Name);
            Assert.Equal(sortedNames[1], resultList[1].Name);
            Assert.Equal(sortedNames[2], resultList[2].Name);
        }

    }
}