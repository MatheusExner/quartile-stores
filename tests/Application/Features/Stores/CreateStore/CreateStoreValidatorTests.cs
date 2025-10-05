using Application.Features.Stores.CreateStore;
using FluentValidation.TestHelper;

namespace tests.Application.Features.Stores.CreateStore
{
    public class CreateStoreValidatorTest
    {
        private readonly CreateStoreValidator _validator;

        public CreateStoreValidatorTest()
        {
            _validator = new CreateStoreValidator();
        }

        [Fact]
        public void Should_Pass_Validation_When_All_Properties_Are_Valid()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Name = string.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Null()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Name = null!;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Name = new string('A', 101);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name must not exceed 100 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_At_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Name = new string('A', 100);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Address_Is_Empty()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Address = string.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Address)
                .WithErrorMessage("Address is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Address_Is_Null()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Address = null!;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Address)
                .WithErrorMessage("Address is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Address_Exceeds_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Address = new string('A', 201);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Address)
                .WithErrorMessage("Address must not exceed 200 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Address_Is_At_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Address = new string('A', 200);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Address);
        }

        [Fact]
        public void Should_Have_Error_When_City_Is_Empty()
        {
            // Arrange
            var command = CreateValidCommand();
            command.City = string.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.City)
                .WithErrorMessage("City is required.");
        }

        [Fact]
        public void Should_Have_Error_When_City_Is_Null()
        {
            // Arrange
            var command = CreateValidCommand();
            command.City = null!;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.City)
                .WithErrorMessage("City is required.");
        }

        [Fact]
        public void Should_Have_Error_When_City_Exceeds_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.City = new string('A', 101);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.City)
                .WithErrorMessage("City must not exceed 100 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_City_Is_At_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.City = new string('A', 100);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.City);
        }

        [Fact]
        public void Should_Have_Error_When_Country_Is_Empty()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Country = string.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Country)
                .WithErrorMessage("Country is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Country_Is_Null()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Country = null!;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Country)
                .WithErrorMessage("Country is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Country_Exceeds_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Country = new string('A', 101);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Country)
                .WithErrorMessage("Country must not exceed 100 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Country_Is_At_Maximum_Length()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Country = new string('A', 100);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Country);
        }

        [Fact]
        public void Should_Have_Error_When_CompanyId_Is_Empty()
        {
            // Arrange
            var command = CreateValidCommand();
            command.CompanyId = Guid.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CompanyId)
                .WithErrorMessage("CompanyId is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_CompanyId_Is_Valid_Guid()
        {
            // Arrange
            var command = CreateValidCommand();
            command.CompanyId = Guid.NewGuid();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
        }

        private static CreateStoreCommand CreateValidCommand()
        {
            return new CreateStoreCommand
            {
                Name = "Test Store",
                Address = "123 Test Street",
                City = "Test City",
                Country = "Test Country",
                CompanyId = Guid.NewGuid()
            };
        }
    }
}