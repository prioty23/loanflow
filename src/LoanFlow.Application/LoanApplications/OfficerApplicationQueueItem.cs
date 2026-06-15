namespace LoanFlow.Application.LoanApplications;

public sealed record OfficerApplicationQueueItem(
    int Id,
    string SubmissionReference,
    string ApplicantFullName,
    string ProductName,
    decimal RequestedAmount,
    DateTime SubmittedAtUtc);
