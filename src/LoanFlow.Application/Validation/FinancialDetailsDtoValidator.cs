using FluentValidation;
using LoanFlow.Application.LoanApplications;

namespace LoanFlow.Application.Validation;

public sealed class FinancialDetailsDtoValidator : AbstractValidator<FinancialDetailsDto>
{
    public FinancialDetailsDtoValidator()
    {
        RuleFor(model => model.HousingExpense).GreaterThanOrEqualTo(0);
        RuleFor(model => model.LivingExpense).GreaterThanOrEqualTo(0);
        RuleFor(model => model.ExistingMonthlyEmi).GreaterThanOrEqualTo(0);
        RuleFor(model => model.OtherLiabilities).GreaterThanOrEqualTo(0);
    }
}
