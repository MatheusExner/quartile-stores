using FluentValidation;

namespace Application.Features.Companies.GetCompany
{
    public class GetCompanyValidator : AbstractValidator<GetCompanyQuery>
    {
        public GetCompanyValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}