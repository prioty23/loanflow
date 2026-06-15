namespace LoanFlow.Application.LoanApplications;

public sealed record LoanApplicationTimelineItem(
    string EventName,
    DateTime OccurredAtUtc,
    string Description);
