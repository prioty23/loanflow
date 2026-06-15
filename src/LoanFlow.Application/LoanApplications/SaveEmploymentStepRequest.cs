using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.LoanApplications;

public sealed record SaveEmploymentStepRequest(
    EmploymentType EmploymentType,
    string EmployerOrBusinessName,
    string JobTitle,
    DateOnly EmploymentStartDate,
    decimal MonthlyNetSalary,
    decimal OtherMonthlyIncome);
