namespace LoanFlow.Application.LoanApplications;

public sealed class SubmitLoanApplicationCommand
{
    public CustomerProfiles.CustomerProfileDto? CustomerProfile { get; set; }

    public EmploymentDetailsDto EmploymentDetails { get; set; } = new();

    public FinancialDetailsDto FinancialDetails { get; set; } = new();

    public LoanRequestDto LoanRequest { get; set; } = new();

    public bool DeclarationAccepted { get; set; }

    public bool ProductIsActive { get; set; }

    public decimal ProductMinimumLoanAmount { get; set; }

    public decimal ProductMaximumLoanAmount { get; set; }

    public int ProductMinimumTenureMonths { get; set; }

    public int ProductMaximumTenureMonths { get; set; }
}
