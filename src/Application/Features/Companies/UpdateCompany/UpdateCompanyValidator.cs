using FluentValidation;

namespace Application.Features.Companies.UpdateCompany
{
    public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}