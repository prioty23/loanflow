namespace LoanFlow.Application.LoanApplications;

public sealed record SaveMonthlyFinancesStepRequest(
    decimal HousingExpense,
    decimal LivingExpense,
    decimal ExistingMonthlyEmi,
    decimal OtherLiabilities);
