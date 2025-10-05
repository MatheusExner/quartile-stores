using FluentValidation;

namespace Application.Features.Stores.DeleteStore
{
    public class DeleteStoreValidator : AbstractValidator<DeleteStoreCommand>
    {
        public DeleteStoreValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}