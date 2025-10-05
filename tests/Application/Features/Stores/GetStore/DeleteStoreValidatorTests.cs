using Application.Features.Stores.DeleteStore;
using Application.Features.Stores.GetStore;
using FluentValidation.TestHelper;

namespace tests.Application.Features.Stores.GetStore
{
    public class GetStoreValidatorTest
    {
        private readonly GetStoreValidator _validator;

        public GetStoreValidatorTest()
        {
            _validator = new GetStoreValidator();
        }

        [Fact]
        public void Should_Pass_Validation_When_Id_Is_Valid()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Id = Guid.Empty;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Id is required.");
        }


        private static GetStoreQuery CreateValidCommand()
        {
            return new GetStoreQuery
            {
                Id = Guid.NewGuid()
            };
        }
    }
}