using LoanFlow.Application.LoanApplications;
using LoanFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Persistence;

public sealed class LoanApplicationReadService(ApplicationDbContext dbContext) : ILoanApplicationReadService
{
    public async Task<SubmittedLoanApplicationDetails?> GetSubmittedDetailsForCustomerAsync(
        string customerUserId,
        int applicationId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.LoanApplications
            .AsNoTracking()
            .Where(application =>
                application.CustomerUserId == customerUserId &&
                application.Id == applicationId &&
                application.Status == ApplicationStatus.Submitted)
            .Join(
                dbContext.LoanProducts.AsNoTracking(),
                application => application.LoanProductId,
                product => product.Id,
                (application, product) => new { application, product })
            .Select(item => new SubmittedLoanApplicationDetails(
                item.application.Id,
                item.application.SubmissionReference ?? "",
                item.application.ApplicantFullName,
                item.product.ProductName,
                item.product.ProductCode,
                item.application.RequestedAmount,
                item.application.RequestedTenureMonths,
                item.application.LoanPurpose,
                item.application.PurposeDescription,
                item.application.SubmittedAtUtc ?? item.application.UpdatedAtUtc,
                item.application.ApplicantSnapshot!.FullName,
                item.application.ApplicantSnapshot.DateOfBirth,
                item.application.ApplicantSnapshot.NationalIdNumber,
                item.application.ApplicantSnapshot.MobileNumber,
                item.application.ApplicantSnapshot.CurrentAddress,
                item.application.ApplicantSnapshot.PermanentAddress,
                item.application.FinancialProfile!.EmploymentType,
                item.application.FinancialProfile.EmployerOrBusinessName,
                item.application.FinancialProfile.JobTitle,
                item.application.FinancialProfile.EmploymentStartDate,
                item.application.FinancialProfile.MonthlyNetSalary,
                item.application.FinancialProfile.OtherMonthlyIncome,
                item.application.FinancialProfile.TotalMonthlyIncome,
                item.application.FinancialProfile.HousingExpense,
                item.application.FinancialProfile.LivingExpense,
                item.application.FinancialProfile.ExistingMonthlyEmi,
                item.application.FinancialProfile.OtherLiabilities,
                item.application.FinancialProfile.TotalMonthlyExpenses,
                item.application.FinancialProfile.CalculatedDisposableIncome))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LoanApplicationTimelineItem>> GetTimelineForCustomerAsync(
        string customerUserId,
        int applicationId,
        CancellationToken cancellationToken = default)
    {
        var application = await dbContext.LoanApplications
            .AsNoTracking()
            .Include(existingApplication => existingApplication.StatusHistory)
            .SingleOrDefaultAsync(
                existingApplication =>
                    existingApplication.CustomerUserId == customerUserId &&
                    existingApplication.Id == applicationId &&
                    existingApplication.Status == ApplicationStatus.Submitted,
                cancellationToken);

        if (application is null)
        {
            return [];
        }

        var items = new List<LoanApplicationTimelineItem>
        {
            new(
                "Draft",
                application.CreatedAtUtc,
                "The customer started the application draft.")
        };

        items.AddRange(
            application.StatusHistory
                .OrderBy(history => history.ChangedAtUtc)
                .Select(history => new LoanApplicationTimelineItem(
                    history.ToStatus.ToString(),
                    history.ChangedAtUtc,
                    history.Note ?? $"Status changed from {history.FromStatus} to {history.ToStatus}.")));

        return items
            .OrderBy(item => item.OccurredAtUtc)
            .ToList();
    }

    public async Task<IReadOnlyList<OfficerApplicationQueueItem>> GetSubmittedQueueAsync(
        string? referenceSearch,
        bool newestFirst,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.LoanApplications
            .AsNoTracking()
            .Where(application => application.Status == ApplicationStatus.Submitted);

        if (!string.IsNullOrWhiteSpace(referenceSearch))
        {
            var trimmedSearch = referenceSearch.Trim();
            query = query.Where(application =>
                application.SubmissionReference != null &&
                application.SubmissionReference.Contains(trimmedSearch));
        }

        query = newestFirst
            ? query.OrderByDescending(application => application.SubmittedAtUtc)
            : query.OrderBy(application => application.SubmittedAtUtc);

        return await query
            .Join(
                dbContext.LoanProducts.AsNoTracking(),
                application => application.LoanProductId,
                product => product.Id,
                (application, product) => new OfficerApplicationQueueItem(
                    application.Id,
                    application.SubmissionReference ?? "",
                    application.ApplicantFullName,
                    product.ProductName,
                    application.RequestedAmount,
                    application.SubmittedAtUtc ?? application.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
