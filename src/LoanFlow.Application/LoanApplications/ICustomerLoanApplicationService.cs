namespace LoanFlow.Application.LoanApplications;

public interface ICustomerLoanApplicationService
{
    Task<IReadOnlyList<CustomerLoanApplicationListItem>> GetForCustomerAsync(
        string customerUserId,
        CancellationToken cancellationToken = default);

    Task<int> CreateDraftAsync(string customerUserId, CancellationToken cancellationToken = default);

    Task<CustomerLoanApplicationWizard?> GetWizardAsync(
        string customerUserId,
        int applicationId,
        CancellationToken cancellationToken = default);

    Task SaveEmploymentStepAsync(
        string customerUserId,
        int applicationId,
        SaveEmploymentStepRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default);

    Task SaveMonthlyFinancesStepAsync(
        string customerUserId,
        int applicationId,
        SaveMonthlyFinancesStepRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default);

    Task SaveLoanRequestStepAsync(
        string customerUserId,
        int applicationId,
        SaveCustomerLoanApplicationRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default);

    Task SaveReviewStepAsync(
        string customerUserId,
        int applicationId,
        SaveReviewStepRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default);

    Task SubmitAsync(
        string customerUserId,
        int applicationId,
        bool declarationAccepted,
        CancellationToken cancellationToken = default);
}
