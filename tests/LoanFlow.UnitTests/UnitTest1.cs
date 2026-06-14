using LoanFlow.Application.Authorization;
using LoanFlow.Domain.Entities;

namespace LoanFlow.UnitTests;

public class UnitTest1
{
    [Fact]
    public void AppRoles_All_ContainsTheExpectedRoles()
    {
        Assert.Equal(
            [AppRoles.Customer, AppRoles.LoanOfficer, AppRoles.Approver, AppRoles.Administrator],
            AppRoles.All);
    }

    [Fact]
    public void LoanProduct_StoresTheExpectedConfiguration()
    {
        var product = new LoanProduct(
            productName: "Everyday Personal Loan",
            productCode: "pl-start",
            minimumLoanAmount: 50000m,
            maximumLoanAmount: 500000m,
            annualInterestRate: 11.90m,
            minimumTenureMonths: 12,
            maximumTenureMonths: 60,
            minimumMonthlyIncome: 25000m,
            maximumDebtToIncomeRatio: 0.45m,
            isActive: true);

        Assert.Equal("Everyday Personal Loan", product.ProductName);
        Assert.Equal("PL-START", product.ProductCode);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void LoanProduct_Throws_WhenMaximumLoanAmountIsLowerThanMinimumLoanAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LoanProduct(
            productName: "Everyday Personal Loan",
            productCode: "PL-START",
            minimumLoanAmount: 100000m,
            maximumLoanAmount: 50000m,
            annualInterestRate: 11.90m,
            minimumTenureMonths: 12,
            maximumTenureMonths: 60,
            minimumMonthlyIncome: 25000m,
            maximumDebtToIncomeRatio: 0.45m,
            isActive: true));
    }

    [Fact]
    public void LoanProduct_Throws_WhenDebtToIncomeRatioIsGreaterThanOne()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LoanProduct(
            productName: "Everyday Personal Loan",
            productCode: "PL-START",
            minimumLoanAmount: 50000m,
            maximumLoanAmount: 500000m,
            annualInterestRate: 11.90m,
            minimumTenureMonths: 12,
            maximumTenureMonths: 60,
            minimumMonthlyIncome: 25000m,
            maximumDebtToIncomeRatio: 1.10m,
            isActive: true));
    }
}
