using LoanFlow.Application.LoanProducts;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Persistence;

public sealed class LoanProductReadService(ApplicationDbContext dbContext) : ILoanProductReadService
{
    public async Task<IReadOnlyList<LoanProductSummary>> GetLoanProductsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.LoanProducts
            .AsNoTracking()
            .OrderBy(product => product.ProductName)
            .Select(product => new LoanProductSummary(
                product.ProductName,
                product.ProductCode,
                product.MinimumLoanAmount,
                product.MaximumLoanAmount,
                product.AnnualInterestRate,
                product.MinimumTenureMonths,
                product.MaximumTenureMonths,
                product.MinimumMonthlyIncome,
                product.MaximumDebtToIncomeRatio,
                product.IsActive))
            .ToListAsync(cancellationToken);
    }
}
