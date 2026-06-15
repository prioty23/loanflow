using FluentValidation;
using LoanFlow.Application.LoanApplications;

namespace LoanFlow.Application.Validation;

public sealed class EmploymentDetailsDtoValidator : AbstractValidator<EmploymentDetailsDto>
{
    public EmploymentDetailsDtoValidator()
    {
        RuleFor(model => model.EmployerOrBusinessName)
            .MaximumLength(150);

        RuleFor(model => model.JobTitle)
            .MaximumLength(100);

        RuleFor(model => model.EmploymentStartDate)
            .Must(date => date is null || date.Value <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Employment start date cannot be in the future.");

        RuleFor(model => model.MonthlyNetSalary)
            .GreaterThanOrEqualTo(0);

        RuleFor(model => model.OtherMonthlyIncome)
            .GreaterThanOrEqualTo(0);
    }
}
