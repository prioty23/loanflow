namespace LoanFlow.Application.LoanProducts;

public sealed record LoanProductSummary(
    string ProductName,
    string ProductCode,
    decimal MinimumLoanAmount,
    decimal MaximumLoanAmount,
    decimal AnnualInterestRate,
    int MinimumTenureMonths,
    int MaximumTenureMonths,
    decimal MinimumMonthlyIncome,
    decimal MaximumDebtToIncomeRatio,
    bool IsActive);
