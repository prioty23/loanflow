namespace LoanFlow.Domain.Entities;

public class LoanProduct
{
    public int Id { get; private set; }

    public string ProductName { get; private set; } = "";

    public string ProductCode { get; private set; } = "";

    public decimal MinimumLoanAmount { get; private set; }

    public decimal MaximumLoanAmount { get; private set; }

    public decimal AnnualInterestRate { get; private set; }

    public int MinimumTenureMonths { get; private set; }

    public int MaximumTenureMonths { get; private set; }

    public decimal MinimumMonthlyIncome { get; private set; }

    public decimal MaximumDebtToIncomeRatio { get; private set; }

    public bool IsActive { get; private set; }

    private LoanProduct()
    {
    }

    public LoanProduct(
        string productName,
        string productCode,
        decimal minimumLoanAmount,
        decimal maximumLoanAmount,
        decimal annualInterestRate,
        int minimumTenureMonths,
        int maximumTenureMonths,
        decimal minimumMonthlyIncome,
        decimal maximumDebtToIncomeRatio,
        bool isActive)
    {
        Update(
            productName,
            productCode,
            minimumLoanAmount,
            maximumLoanAmount,
            annualInterestRate,
            minimumTenureMonths,
            maximumTenureMonths,
            minimumMonthlyIncome,
            maximumDebtToIncomeRatio,
            isActive);
    }

    public void Update(
        string productName,
        string productCode,
        decimal minimumLoanAmount,
        decimal maximumLoanAmount,
        decimal annualInterestRate,
        int minimumTenureMonths,
        int maximumTenureMonths,
        decimal minimumMonthlyIncome,
        decimal maximumDebtToIncomeRatio,
        bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);

        if (minimumLoanAmount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumLoanAmount));
        }

        if (maximumLoanAmount < minimumLoanAmount)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumLoanAmount));
        }

        if (annualInterestRate < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(annualInterestRate));
        }

        if (minimumTenureMonths <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumTenureMonths));
        }

        if (maximumTenureMonths < minimumTenureMonths)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumTenureMonths));
        }

        if (minimumMonthlyIncome < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumMonthlyIncome));
        }

        if (maximumDebtToIncomeRatio < 0 || maximumDebtToIncomeRatio > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumDebtToIncomeRatio));
        }

        ProductName = productName.Trim();
        ProductCode = productCode.Trim().ToUpperInvariant();
        MinimumLoanAmount = minimumLoanAmount;
        MaximumLoanAmount = maximumLoanAmount;
        AnnualInterestRate = annualInterestRate;
        MinimumTenureMonths = minimumTenureMonths;
        MaximumTenureMonths = maximumTenureMonths;
        MinimumMonthlyIncome = minimumMonthlyIncome;
        MaximumDebtToIncomeRatio = maximumDebtToIncomeRatio;
        IsActive = isActive;
    }
}
