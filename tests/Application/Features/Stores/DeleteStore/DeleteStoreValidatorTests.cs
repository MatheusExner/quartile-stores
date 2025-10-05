using Application.Features.Stores.DeleteStore;
using FluentValidation.TestHelper;

namespace tests.Application.Features.Stores.DeleteStore
{
    public class DeleteStoreValidatorTest
    {
        private readonly DeleteStoreValidator _validator;

        public DeleteStoreValidatorTest()
        {
            _validator = new DeleteStoreValidator();
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


        private static DeleteStoreCommand CreateValidCommand()
        {
            return new DeleteStoreCommand
            {
                Id = Guid.NewGuid()
            };
        }
    }
}