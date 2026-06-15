using FluentValidation;
using LoanFlow.Application.LoanApplications;
using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.Validation;

public sealed class SubmitLoanApplicationCommandValidator : AbstractValidator<SubmitLoanApplicationCommand>
{
    public SubmitLoanApplicationCommandValidator()
    {
        RuleFor(model => model.CustomerProfile)
            .NotNull()
            .WithMessage("Customer profile is required.");

        When(model => model.CustomerProfile is not null, () =>
        {
            RuleFor(model => model.CustomerProfile!)
                .SetValidator(new CustomerProfileDtoValidator());
        });

        RuleFor(model => model.EmploymentDetails)
            .SetValidator(new EmploymentDetailsDtoValidator());

        RuleFor(model => model.EmploymentDetails.JobTitle)
            .NotEmpty()
            .WithMessage("Job title is required.");

        RuleFor(model => model.EmploymentDetails.EmploymentStartDate)
            .NotNull()
            .WithMessage("Employment start date is required.");

        RuleFor(model => model.EmploymentDetails.MonthlyNetSalary)
            .GreaterThan(0)
            .WithMessage("Monthly net salary is required.");

        RuleFor(model => model.EmploymentDetails.EmployerOrBusinessName)
            .NotEmpty()
            .When(model => model.EmploymentDetails.EmploymentType == EmploymentType.Salaried)
            .WithMessage("Employer or business name is required for salaried applicants.");

        RuleFor(model => model.FinancialDetails)
            .SetValidator(new FinancialDetailsDtoValidator());

        RuleFor(model => model.LoanRequest)
            .SetValidator(new LoanRequestDtoValidator());

        RuleFor(model => model.LoanRequest.ApplicantFullName)
            .NotEmpty()
            .WithMessage("Full name is required.");

        RuleFor(model => model.LoanRequest.RequestedAmount)
            .GreaterThan(0)
            .WithMessage("Requested amount is required.");

        RuleFor(model => model.LoanRequest.RequestedTenureMonths)
            .GreaterThan(0)
            .WithMessage("Requested tenure is required.");

        RuleFor(model => model.LoanRequest.PurposeDescription)
            .NotEmpty()
            .When(model => model.LoanRequest.LoanPurpose == LoanPurpose.Other)
            .WithMessage("Purpose description is required when loan purpose is Other.");

        RuleFor(model => model.ProductIsActive)
            .Equal(true)
            .WithMessage("The selected loan product is no longer active.");

        RuleFor(model => model.LoanRequest.RequestedAmount)
            .Must((model, value) =>
                value >= model.ProductMinimumLoanAmount &&
                value <= model.ProductMaximumLoanAmount)
            .When(model => model.ProductIsActive)
            .WithMessage(model =>
                $"Requested amount must be between {model.ProductMinimumLoanAmount:0.##} and {model.ProductMaximumLoanAmount:0.##}.");

        RuleFor(model => model.LoanRequest.RequestedTenureMonths)
            .Must((model, value) =>
                value >= model.ProductMinimumTenureMonths &&
                value <= model.ProductMaximumTenureMonths)
            .When(model => model.ProductIsActive)
            .WithMessage(model =>
                $"Requested tenure must be between {model.ProductMinimumTenureMonths} and {model.ProductMaximumTenureMonths} months.");

        RuleFor(model => model.DeclarationAccepted)
            .Equal(true)
            .WithMessage("You must accept the declaration before submitting.");
    }
}
