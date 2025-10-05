using FluentValidation;

namespace Application.Features.Stores.GetStore
{
    public class GetStoreValidator : AbstractValidator<GetStoreQuery>
    {
        public GetStoreValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}