using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.LoanApplications;

public sealed record SubmittedLoanApplicationDetails(
    int Id,
    string SubmissionReference,
    string ApplicantFullName,
    string ProductName,
    string ProductCode,
    decimal RequestedAmount,
    int RequestedTenureMonths,
    LoanPurpose LoanPurpose,
    string PurposeDescription,
    DateTime SubmittedAtUtc,
    string ProfileFullName,
    DateOnly ProfileDateOfBirth,
    string ProfileNationalIdNumber,
    string ProfileMobileNumber,
    string ProfileCurrentAddress,
    string ProfilePermanentAddress,
    EmploymentType EmploymentType,
    string EmployerOrBusinessName,
    string JobTitle,
    DateOnly EmploymentStartDate,
    decimal MonthlyNetSalary,
    decimal OtherMonthlyIncome,
    decimal TotalMonthlyIncome,
    decimal HousingExpense,
    decimal LivingExpense,
    decimal ExistingMonthlyEmi,
    decimal OtherLiabilities,
    decimal TotalMonthlyExpenses,
    decimal CalculatedDisposableIncome);
