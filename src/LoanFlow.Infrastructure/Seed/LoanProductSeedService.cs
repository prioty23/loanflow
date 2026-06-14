using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LoanFlow.Infrastructure.Seed;

public static class LoanProductSeedService
{
    private const string ProductCode = "PL-START";

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var loanProduct = await dbContext.LoanProducts
            .SingleOrDefaultAsync(product => product.ProductCode == ProductCode, cancellationToken);

        if (loanProduct is null)
        {
            loanProduct = new LoanProduct(
                productName: "Everyday Personal Loan",
                productCode: ProductCode,
                minimumLoanAmount: 50000m,
                maximumLoanAmount: 500000m,
                annualInterestRate: 11.90m,
                minimumTenureMonths: 12,
                maximumTenureMonths: 60,
                minimumMonthlyIncome: 25000m,
                maximumDebtToIncomeRatio: 0.45m,
                isActive: true);

            dbContext.LoanProducts.Add(loanProduct);
        }
        else
        {
            loanProduct.Update(
                productName: "Everyday Personal Loan",
                productCode: ProductCode,
                minimumLoanAmount: 50000m,
                maximumLoanAmount: 500000m,
                annualInterestRate: 11.90m,
                minimumTenureMonths: 12,
                maximumTenureMonths: 60,
                minimumMonthlyIncome: 25000m,
                maximumDebtToIncomeRatio: 0.45m,
                isActive: true);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
