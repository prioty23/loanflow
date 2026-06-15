using LoanFlow.Domain.Enums;

namespace LoanFlow.Domain.Entities;

public class LoanApplicationFinancialProfile
{
    public int Id { get; private set; }

    public int LoanApplicationId { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public EmploymentType EmploymentType { get; private set; }

    public string EmployerOrBusinessName { get; private set; } = "";

    public string JobTitle { get; private set; } = "";

    public DateOnly EmploymentStartDate { get; private set; }

    public decimal MonthlyNetSalary { get; private set; }

    public decimal OtherMonthlyIncome { get; private set; }

    public decimal HousingExpense { get; private set; }

    public decimal LivingExpense { get; private set; }

    public decimal ExistingMonthlyEmi { get; private set; }

    public decimal OtherLiabilities { get; private set; }

    public decimal TotalMonthlyIncome { get; private set; }

    public decimal TotalMonthlyExpenses { get; private set; }

    public decimal CalculatedDisposableIncome { get; private set; }

    private LoanApplicationFinancialProfile()
    {
    }

    public LoanApplicationFinancialProfile(
        EmploymentType employmentType,
        string employerOrBusinessName,
        string jobTitle,
        DateOnly employmentStartDate,
        decimal monthlyNetSalary,
        decimal otherMonthlyIncome)
    {
        UpdateEmployment(
            employmentType,
            employerOrBusinessName,
            jobTitle,
            employmentStartDate,
            monthlyNetSalary,
            otherMonthlyIncome);
    }

    public void UpdateEmployment(
        EmploymentType employmentType,
        string employerOrBusinessName,
        string jobTitle,
        DateOnly employmentStartDate,
        decimal monthlyNetSalary,
        decimal otherMonthlyIncome)
    {
        if (employmentStartDate > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ArgumentException("Employment start date cannot be in the future.", nameof(employmentStartDate));
        }

        if (jobTitle is null)
        {
            throw new ArgumentNullException(nameof(jobTitle));
        }

        if (monthlyNetSalary < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monthlyNetSalary));
        }

        if (otherMonthlyIncome < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(otherMonthlyIncome));
        }

        EmploymentType = employmentType;
        EmployerOrBusinessName = employerOrBusinessName?.Trim() ?? "";
        JobTitle = jobTitle.Trim();
        EmploymentStartDate = employmentStartDate;
        MonthlyNetSalary = monthlyNetSalary;
        OtherMonthlyIncome = otherMonthlyIncome;

        Recalculate();
    }

    public void UpdateMonthlyFinances(
        decimal housingExpense,
        decimal livingExpense,
        decimal existingMonthlyEmi,
        decimal otherLiabilities)
    {
        if (housingExpense < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(housingExpense));
        }

        if (livingExpense < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(livingExpense));
        }

        if (existingMonthlyEmi < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(existingMonthlyEmi));
        }

        if (otherLiabilities < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(otherLiabilities));
        }

        HousingExpense = housingExpense;
        LivingExpense = livingExpense;
        ExistingMonthlyEmi = existingMonthlyEmi;
        OtherLiabilities = otherLiabilities;

        Recalculate();
    }

    private void Recalculate()
    {
        TotalMonthlyIncome = MonthlyNetSalary + OtherMonthlyIncome;
        TotalMonthlyExpenses = HousingExpense + LivingExpense + ExistingMonthlyEmi + OtherLiabilities;
        CalculatedDisposableIncome = TotalMonthlyIncome - TotalMonthlyExpenses;
    }
}
