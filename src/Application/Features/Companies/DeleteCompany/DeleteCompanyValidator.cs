using FluentValidation;

namespace Application.Features.Companies.DeleteCompany
{
    public class DeleteCompanyValidator : AbstractValidator<DeleteCompanyCommand>
    {
        public DeleteCompanyValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}