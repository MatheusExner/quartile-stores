using Domain.Entities;

namespace tests.Domain.Entities
{
    public class CompanyTest
    {
        [Fact]
        public void Should_Initialize_With_Default_Values_When_Created()
        {
            // Act
            var company = new Company();

            // Assert
            Assert.Equal(string.Empty, company.Name);
            Assert.NotNull(company.Stores);
            Assert.Empty(company.Stores);
        }

        [Fact]
        public void Should_Set_Name_Property()
        {
            // Arrange
            var company = new Company()
            {
                Name = "Initial Name"
            };

            // Act

            // Assert
            Assert.Equal("Initial Name", company.Name);
        }

    }
}