using Domain.Entities;

namespace tests.Domain.Entities
{
    public class ProductTests
    {
        [Fact]
        public void Should_Initialize_With_Default_Values_When_Created()
        {
            // Act
            var product = new Product();

            // Assert
            Assert.Equal(string.Empty, product.Name);
            Assert.Equal(string.Empty, product.Description);
            Assert.Equal(0, product.Price);
            Assert.NotNull(product.Store);
        }

        [Fact]
        public void Should_Set_Properties()
        {
            // Arrange
            var product = new Product()
            {
                Name = "Initial Name",
                Description = "Initial Description",
                Price = 10.0m
            };

            // Act

            // Assert
            Assert.Equal("Initial Name", product.Name);
            Assert.Equal("Initial Description", product.Description);
            Assert.Equal(10.0m, product.Price);
        }
    }
}