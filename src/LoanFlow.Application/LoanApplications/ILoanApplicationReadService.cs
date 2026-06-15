namespace LoanFlow.Application.LoanApplications;

public interface ILoanApplicationReadService
{
    Task<SubmittedLoanApplicationDetails?> GetSubmittedDetailsForCustomerAsync(
        string customerUserId,
        int applicationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LoanApplicationTimelineItem>> GetTimelineForCustomerAsync(
        string customerUserId,
        int applicationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OfficerApplicationQueueItem>> GetSubmittedQueueAsync(
        string? referenceSearch,
        bool newestFirst,
        CancellationToken cancellationToken = default);
}
