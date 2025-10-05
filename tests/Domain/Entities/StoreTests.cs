using Domain.Entities;

namespace tests.Domain.Entities
{
    public class StoreTests
    {
        [Fact]
        public void Should_Initialize_With_Default_Values_When_Created()
        {
            // Act
            var store = new Store();

            // Assert
            Assert.Equal(string.Empty, store.Name);
            Assert.Equal(string.Empty, store.Address);
            Assert.Equal(string.Empty, store.City);
            Assert.Equal(string.Empty, store.Country);
        }

        [Fact]
        public void Should_Set_Properties()
        {
            // Arrange
            var store = new Store()
            {
                Name = "Initial Name",
                Address = "Initial Address",
                City = "Initial City",
                Country = "Initial Country"
            };


            // Assert
            Assert.Equal("Initial Name", store.Name);
            Assert.Equal("Initial Address", store.Address);
            Assert.Equal("Initial City", store.City);
            Assert.Equal("Initial Country", store.Country);
        }
    }
}