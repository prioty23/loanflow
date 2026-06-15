using FluentValidation;
using LoanFlow.Application.LoanApplications;

namespace LoanFlow.Application.Validation;

public sealed class LoanRequestDtoValidator : AbstractValidator<LoanRequestDto>
{
    public LoanRequestDtoValidator()
    {
        RuleFor(model => model.ApplicantFullName)
            .MaximumLength(150);

        RuleFor(model => model.RequestedAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(model => model.RequestedTenureMonths)
            .GreaterThanOrEqualTo(0);

        RuleFor(model => model.PurposeDescription)
            .MaximumLength(250);
    }
}
