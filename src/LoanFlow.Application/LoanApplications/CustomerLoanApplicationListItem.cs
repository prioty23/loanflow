using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.LoanApplications;

public sealed record CustomerLoanApplicationListItem(
    int Id,
    string ApplicantFullName,
    string ProductName,
    ApplicationStatus Status,
    DateTime UpdatedAtUtc);
