namespace LoanFlow.Application.LoanProducts;

public interface ILoanProductReadService
{
    Task<IReadOnlyList<LoanProductSummary>> GetLoanProductsAsync(CancellationToken cancellationToken = default);
}
