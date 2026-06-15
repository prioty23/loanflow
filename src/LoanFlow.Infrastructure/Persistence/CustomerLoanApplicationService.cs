using FluentValidation;
using LoanFlow.Application.CustomerProfiles;
using LoanFlow.Application.LoanApplications;
using LoanFlow.Domain.Entities;
using LoanFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Persistence;

public sealed class CustomerLoanApplicationService(
    ApplicationDbContext dbContext,
    IValidator<SubmitLoanApplicationCommand> submitValidator) : ICustomerLoanApplicationService
{
    public async Task<IReadOnlyList<CustomerLoanApplicationListItem>> GetForCustomerAsync(
        string customerUserId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.LoanApplications
            .AsNoTracking()
            .Where(application => application.CustomerUserId == customerUserId)
            .OrderByDescending(application => application.UpdatedAtUtc)
            .Select(application => new CustomerLoanApplicationListItem(
                application.Id,
                application.ApplicantFullName,
                dbContext.LoanProducts
                    .Where(product => product.Id == application.LoanProductId)
                    .Select(product => product.ProductName)
                    .FirstOrDefault() ?? "Unknown product",
                application.Status,
                application.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CreateDraftAsync(string customerUserId, CancellationToken cancellationToken = default)
    {
        var activeProduct = await dbContext.LoanProducts
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.ProductName)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeProduct is null)
        {
            throw new InvalidOperationException("No active loan product is available right now.");
        }

        var draftApplication = new LoanApplication(
            customerUserId: customerUserId,
            loanProductId: activeProduct.Id,
            applicantFullName: "New Customer",
            requestedAmount: activeProduct.MinimumLoanAmount,
            requestedTenureMonths: activeProduct.MinimumTenureMonths,
            monthlyIncome: activeProduct.MinimumMonthlyIncome,
            employmentType: EmploymentType.Salaried,
            loanPurpose: LoanPurpose.PersonalExpense);

        dbContext.LoanApplications.Add(draftApplication);
        await dbContext.SaveChangesAsync(cancellationToken);

        return draftApplication.Id;
    }

    public async Task<CustomerLoanApplicationWizard?> GetWizardAsync(
        string customerUserId,
        int applicationId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.LoanApplications
            .AsNoTracking()
            .Where(application => application.CustomerUserId == customerUserId && application.Id == applicationId)
            .Join(
                dbContext.LoanProducts.AsNoTracking(),
                application => application.LoanProductId,
                product => product.Id,
                (application, product) => new { application, product })
            .Select(item => new CustomerLoanApplicationWizard(
                item.application.Id,
                item.product.ProductName,
                item.product.ProductCode,
                item.application.Status,
                GetResumeStep(item.application),
                item.application.LastCompletedStep == 0 ? null : MapStep(item.application.LastCompletedStep),
                item.application.FinancialProfile == null ? item.application.EmploymentType : item.application.FinancialProfile.EmploymentType,
                item.application.FinancialProfile == null ? "" : item.application.FinancialProfile.EmployerOrBusinessName,
                item.application.FinancialProfile == null ? "" : item.application.FinancialProfile.JobTitle,
                item.application.FinancialProfile == null || item.application.FinancialProfile.EmploymentStartDate == DateOnly.MinValue
                    ? null
                    : item.application.FinancialProfile.EmploymentStartDate,
                item.application.FinancialProfile == null ? item.application.MonthlyIncome : item.application.FinancialProfile.MonthlyNetSalary,
                item.application.FinancialProfile == null ? 0m : item.application.FinancialProfile.OtherMonthlyIncome,
                item.application.FinancialProfile == null ? item.application.MonthlyIncome : item.application.FinancialProfile.TotalMonthlyIncome,
                item.application.FinancialProfile == null ? 0m : item.application.FinancialProfile.HousingExpense,
                item.application.FinancialProfile == null ? 0m : item.application.FinancialProfile.LivingExpense,
                item.application.FinancialProfile == null ? 0m : item.application.FinancialProfile.ExistingMonthlyEmi,
                item.application.FinancialProfile == null ? 0m : item.application.FinancialProfile.OtherLiabilities,
                item.application.FinancialProfile == null ? 0m : item.application.FinancialProfile.TotalMonthlyExpenses,
                item.application.FinancialProfile == null ? item.application.MonthlyIncome : item.application.FinancialProfile.CalculatedDisposableIncome,
                item.application.ApplicantFullName,
                item.application.RequestedAmount,
                item.application.RequestedTenureMonths,
                item.application.LoanPurpose,
                item.application.PurposeDescription,
                item.product.IsActive,
                item.product.MinimumLoanAmount,
                item.product.MaximumLoanAmount,
                item.product.MinimumTenureMonths,
                item.product.MaximumTenureMonths,
                item.application.DeclarationAccepted))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task SaveEmploymentStepAsync(
        string customerUserId,
        int applicationId,
        SaveEmploymentStepRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default)
    {
        var application = await GetOwnedApplicationAsync(customerUserId, applicationId, cancellationToken);

        application.SetEmploymentInformation(
            request.EmploymentType,
            request.EmployerOrBusinessName,
            request.JobTitle,
            request.EmploymentStartDate,
            request.MonthlyNetSalary,
            request.OtherMonthlyIncome);

        if (markStepCompleted)
        {
            application.MarkStepCompleted(LoanApplicationStep.Employment);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveMonthlyFinancesStepAsync(
        string customerUserId,
        int applicationId,
        SaveMonthlyFinancesStepRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default)
    {
        var application = await GetOwnedApplicationAsync(customerUserId, applicationId, cancellationToken);

        application.SetMonthlyFinances(
            request.HousingExpense,
            request.LivingExpense,
            request.ExistingMonthlyEmi,
            request.OtherLiabilities);

        if (markStepCompleted)
        {
            application.MarkStepCompleted(LoanApplicationStep.MonthlyFinances);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveLoanRequestStepAsync(
        string customerUserId,
        int applicationId,
        SaveCustomerLoanApplicationRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default)
    {
        var application = await GetOwnedApplicationAsync(customerUserId, applicationId, cancellationToken);
        var loanProduct = await dbContext.LoanProducts
            .SingleAsync(product => product.Id == application.LoanProductId, cancellationToken);

        loanProduct.ValidateLoanRequest(request.RequestedAmount, request.RequestedTenureMonths);

        application.UpdateDraftDetails(
            request.ApplicantFullName,
            request.RequestedAmount,
            request.RequestedTenureMonths,
            application.MonthlyIncome,
            application.EmploymentType,
            request.LoanPurpose,
            request.PurposeDescription);

        if (markStepCompleted)
        {
            application.MarkStepCompleted(LoanApplicationStep.LoanRequest);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveReviewStepAsync(
        string customerUserId,
        int applicationId,
        SaveReviewStepRequest request,
        bool markStepCompleted,
        CancellationToken cancellationToken = default)
    {
        var application = await GetOwnedApplicationAsync(customerUserId, applicationId, cancellationToken);

        application.SetDeclarationAccepted(request.DeclarationAccepted);

        if (markStepCompleted && request.DeclarationAccepted)
        {
            application.MarkStepCompleted(LoanApplicationStep.ReviewAndDeclaration);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SubmitAsync(
        string customerUserId,
        int applicationId,
        bool declarationAccepted,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var application = await GetOwnedApplicationAsync(customerUserId, applicationId, cancellationToken);
            var loanProduct = await dbContext.LoanProducts
                .AsNoTracking()
                .SingleAsync(product => product.Id == application.LoanProductId, cancellationToken);

            var customerProfile = await dbContext.CustomerProfiles
                .AsNoTracking()
                .Where(profile => profile.UserId == customerUserId)
                .Select(profile => new CustomerProfileDto
                {
                    FullName = profile.FullName,
                    DateOfBirth = profile.DateOfBirth,
                    NationalIdNumber = profile.NationalIdNumber,
                    MobileNumber = profile.MobileNumber,
                    CurrentAddress = profile.CurrentAddress,
                    PermanentAddress = profile.PermanentAddress,
                    IsPermanentAddressSameAsPresentAddress = profile.CurrentAddress == profile.PermanentAddress
                })
                .SingleOrDefaultAsync(cancellationToken);

            var command = new SubmitLoanApplicationCommand
            {
                CustomerProfile = customerProfile,
                EmploymentDetails = new EmploymentDetailsDto
                {
                    EmploymentType = application.FinancialProfile?.EmploymentType ?? application.EmploymentType,
                    EmployerOrBusinessName = application.FinancialProfile?.EmployerOrBusinessName ?? "",
                    JobTitle = application.FinancialProfile?.JobTitle ?? "",
                    EmploymentStartDate = application.FinancialProfile?.EmploymentStartDate,
                    MonthlyNetSalary = application.FinancialProfile?.MonthlyNetSalary ?? 0m,
                    OtherMonthlyIncome = application.FinancialProfile?.OtherMonthlyIncome ?? 0m
                },
                FinancialDetails = new FinancialDetailsDto
                {
                    HousingExpense = application.FinancialProfile?.HousingExpense ?? 0m,
                    LivingExpense = application.FinancialProfile?.LivingExpense ?? 0m,
                    ExistingMonthlyEmi = application.FinancialProfile?.ExistingMonthlyEmi ?? 0m,
                    OtherLiabilities = application.FinancialProfile?.OtherLiabilities ?? 0m
                },
                LoanRequest = new LoanRequestDto
                {
                    ApplicantFullName = application.ApplicantFullName,
                    RequestedAmount = application.RequestedAmount,
                    RequestedTenureMonths = application.RequestedTenureMonths,
                    LoanPurpose = application.LoanPurpose,
                    PurposeDescription = application.PurposeDescription
                },
                DeclarationAccepted = declarationAccepted,
                ProductIsActive = loanProduct.IsActive,
                ProductMinimumLoanAmount = loanProduct.MinimumLoanAmount,
                ProductMaximumLoanAmount = loanProduct.MaximumLoanAmount,
                ProductMinimumTenureMonths = loanProduct.MinimumTenureMonths,
                ProductMaximumTenureMonths = loanProduct.MaximumTenureMonths
            };

            await submitValidator.ValidateAndThrowAsync(command, cancellationToken);

            var submittedAtUtc = DateTime.UtcNow;

            application.SetDeclarationAccepted(declarationAccepted);
            application.Submit(
                GenerateSubmissionReference(submittedAtUtc),
                new ApplicantSnapshot(
                    customerProfile!.FullName,
                    customerProfile.DateOfBirth!.Value,
                    customerProfile.NationalIdNumber,
                    customerProfile.MobileNumber,
                    customerProfile.CurrentAddress,
                    customerProfile.PermanentAddress),
                submittedAtUtc);
            application.MarkStepCompleted(LoanApplicationStep.ReviewAndDeclaration);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new InvalidOperationException("This application has already been submitted.");
        }
        catch (DbUpdateException exception) when (IsSubmissionReferenceConflict(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new InvalidOperationException("This application has already been submitted.");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<LoanApplication> GetOwnedApplicationAsync(
        string customerUserId,
        int applicationId,
        CancellationToken cancellationToken)
    {
        var application = await dbContext.LoanApplications
            .Include(existingApplication => existingApplication.FinancialProfile)
            .Include(existingApplication => existingApplication.ApplicantSnapshot)
            .SingleOrDefaultAsync(
                existingApplication => existingApplication.CustomerUserId == customerUserId &&
                                       existingApplication.Id == applicationId,
                cancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException("The application could not be found.");
        }

        return application;
    }

    private static LoanApplicationWizardStep GetResumeStep(LoanApplication application)
    {
        return application.LastCompletedStep switch
        {
            < LoanApplicationStep.Employment => LoanApplicationWizardStep.Employment,
            LoanApplicationStep.Employment => LoanApplicationWizardStep.MonthlyFinances,
            LoanApplicationStep.MonthlyFinances => LoanApplicationWizardStep.LoanRequest,
            LoanApplicationStep.LoanRequest => LoanApplicationWizardStep.ReviewAndDeclaration,
            _ => LoanApplicationWizardStep.ReviewAndDeclaration
        };
    }

    private static LoanApplicationWizardStep MapStep(LoanApplicationStep step)
    {
        return (LoanApplicationWizardStep)(int)step;
    }

    private static string GenerateSubmissionReference(DateTime submittedAtUtc)
    {
        var randomCode = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"LF-{submittedAtUtc:yyyyMMdd}-{randomCode}";
    }

    private static bool IsSubmissionReferenceConflict(DbUpdateException exception)
    {
        return exception.InnerException?.Message.Contains("SubmissionReference", StringComparison.OrdinalIgnoreCase) == true;
    }
}
