namespace LoanFlow.Application.LoanApplications;

public sealed class FinancialDetailsDto
{
    public decimal HousingExpense { get; set; }

    public decimal LivingExpense { get; set; }

    public decimal ExistingMonthlyEmi { get; set; }

    public decimal OtherLiabilities { get; set; }
}
