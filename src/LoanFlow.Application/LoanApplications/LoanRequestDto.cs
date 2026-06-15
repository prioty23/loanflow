using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.LoanApplications;

public sealed class LoanRequestDto
{
    public string ApplicantFullName { get; set; } = "";

    public decimal RequestedAmount { get; set; }

    public int RequestedTenureMonths { get; set; }

    public LoanPurpose LoanPurpose { get; set; } = LoanPurpose.PersonalExpense;

    public string PurposeDescription { get; set; } = "";
}
