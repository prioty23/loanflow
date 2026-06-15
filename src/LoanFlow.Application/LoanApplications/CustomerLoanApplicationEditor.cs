using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.LoanApplications;

public sealed record CustomerLoanApplicationEditor(
    int Id,
    string ApplicantFullName,
    decimal RequestedAmount,
    int RequestedTenureMonths,
    decimal MonthlyIncome,
    EmploymentType EmploymentType,
    LoanPurpose LoanPurpose,
    string ProductName,
    string ProductCode,
    string Status);
