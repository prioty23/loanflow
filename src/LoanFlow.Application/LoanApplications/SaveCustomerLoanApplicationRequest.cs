using LoanFlow.Domain.Enums;

namespace LoanFlow.Application.LoanApplications;

public sealed record SaveCustomerLoanApplicationRequest(
    string ApplicantFullName,
    decimal RequestedAmount,
    int RequestedTenureMonths,
    LoanPurpose LoanPurpose,
    string PurposeDescription);
