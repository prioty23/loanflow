using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.LoanApplications;

public sealed class EmploymentDetailsDto
{
    public EmploymentType EmploymentType { get; set; } = EmploymentType.Salaried;

    public string EmployerOrBusinessName { get; set; } = "";

    public string JobTitle { get; set; } = "";

    public DateOnly? EmploymentStartDate { get; set; }

    public decimal MonthlyNetSalary { get; set; }

    public decimal OtherMonthlyIncome { get; set; }
}
