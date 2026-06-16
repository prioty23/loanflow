using FluentValidation;
using LoanFlow.Application.CustomerProfiles;
using LoanFlow.Application.LoanApplications;
using LoanFlow.Application.Validation;
using LoanFlow.Domain.Entities;
using LoanFlow.Domain.Enums;
using LoanFlow.Domain.Exceptions;
using LoanFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LoanFlow.UnitTests;

public class LoanApplicationServiceTests
{
    [Fact]
    public async Task CustomerOwnership_IsEnforced_WhenLoadingWizard()
    {
        await using var dbContext = CreateDbContext();
        var validator = new SubmitLoanApplicationCommandValidator();
        var service = new CustomerLoanApplicationService(dbContext, validator);

        var application = await SeedDraftApplicationAsync(dbContext, "customer-1");

        var wizard = await service.GetWizardAsync("customer-2", application.Id);

        Assert.Null(wizard);
    }

    [Fact]
    public async Task DraftSave_AndResume_ReturnsTheNextStep()
    {
        await using var dbContext = CreateDbContext();
        var validator = new SubmitLoanApplicationCommandValidator();
        var service = new CustomerLoanApplicationService(dbContext, validator);

        var application = await SeedDraftApplicationAsync(dbContext, "customer-1");

        await service.SaveEmploymentStepAsync(
            "customer-1",
            application.Id,
            new SaveEmploymentStepRequest(
                EmploymentType.Salaried,
                "LoanFlow Ltd",
                "Officer",
                new DateOnly(2022, 1, 10),
                40000m,
                5000m),
            markStepCompleted: true);

        var wizard = await service.GetWizardAsync("customer-1", application.Id);

        Assert.NotNull(wizard);
        Assert.Equal(LoanApplicationWizardStep.MonthlyFinances, wizard!.CurrentStep);
        Assert.Equal(LoanApplicationWizardStep.Employment, wizard.LastCompletedStep);
    }

    [Fact]
    public void StepValidation_RejectsFutureEmploymentStartDate()
    {
        var validator = new EmploymentDetailsDtoValidator();
        var model = new EmploymentDetailsDto
        {
            EmploymentType = EmploymentType.Salaried,
            EmployerOrBusinessName = "LoanFlow Ltd",
            JobTitle = "Officer",
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            MonthlyNetSalary = 40000m,
            OtherMonthlyIncome = 5000m
        };

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "EmploymentStartDate");
    }

    [Fact]
    public async Task SuccessfulSubmission_SavesSubmittedState()
    {
        await using var dbContext = CreateDbContext();
        var validator = new SubmitLoanApplicationCommandValidator();
        var service = new CustomerLoanApplicationService(dbContext, validator);

        var application = await SeedCompleteDraftApplicationAsync(dbContext, "customer-1");

        await service.SubmitAsync("customer-1", application.Id, declarationAccepted: true);

        var savedApplication = await dbContext.LoanApplications
            .Include(existingApplication => existingApplication.ApplicantSnapshot)
            .SingleAsync(existingApplication => existingApplication.Id == application.Id);

        Assert.Equal(ApplicationStatus.Submitted, savedApplication.Status);
        Assert.NotNull(savedApplication.SubmittedAtUtc);
        Assert.False(string.IsNullOrWhiteSpace(savedApplication.SubmissionReference));
        Assert.NotNull(savedApplication.ApplicantSnapshot);
    }

    [Fact]
    public async Task InvalidSubmission_IsRejected()
    {
        await using var dbContext = CreateDbContext();
        var validator = new SubmitLoanApplicationCommandValidator();
        var service = new CustomerLoanApplicationService(dbContext, validator);

        var application = await SeedDraftApplicationAsync(dbContext, "customer-1");

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.SubmitAsync("customer-1", application.Id, declarationAccepted: false));
    }

    [Fact]
    public async Task DuplicateSubmission_IsPrevented()
    {
        await using var dbContext = CreateDbContext();
        var validator = new SubmitLoanApplicationCommandValidator();
        var service = new CustomerLoanApplicationService(dbContext, validator);

        var application = await SeedCompleteDraftApplicationAsync(dbContext, "customer-1");

        await service.SubmitAsync("customer-1", application.Id, declarationAccepted: true);

        await Assert.ThrowsAsync<DomainRuleException>(() =>
            service.SubmitAsync("customer-1", application.Id, declarationAccepted: true));
    }

    [Fact]
    public async Task SubmittedApplication_IsReadOnly_InServiceCalls()
    {
        await using var dbContext = CreateDbContext();
        var validator = new SubmitLoanApplicationCommandValidator();
        var service = new CustomerLoanApplicationService(dbContext, validator);

        var application = await SeedCompleteDraftApplicationAsync(dbContext, "customer-1");
        await service.SubmitAsync("customer-1", application.Id, declarationAccepted: true);

        await Assert.ThrowsAsync<DomainRuleException>(() =>
            service.SaveLoanRequestStepAsync(
                "customer-1",
                application.Id,
                new SaveCustomerLoanApplicationRequest(
                    "Updated Name",
                    260000m,
                    36,
                    LoanPurpose.Education,
                    ""),
                markStepCompleted: false));
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options);
    }

    private static async Task<LoanApplication> SeedDraftApplicationAsync(ApplicationDbContext dbContext, string customerUserId)
    {
        var loanProduct = CreateLoanProduct();
        dbContext.LoanProducts.Add(loanProduct);
        await dbContext.SaveChangesAsync();

        var application = new LoanApplication(
            customerUserId,
            loanProductId: loanProduct.Id,
            applicantFullName: "Amina Rahman",
            requestedAmount: 250000m,
            requestedTenureMonths: 36,
            monthlyIncome: 55000m,
            employmentType: EmploymentType.Salaried,
            loanPurpose: LoanPurpose.Education);

        dbContext.LoanApplications.Add(application);
        await dbContext.SaveChangesAsync();
        return application;
    }

    private static async Task<LoanApplication> SeedCompleteDraftApplicationAsync(ApplicationDbContext dbContext, string customerUserId)
    {
        var loanProduct = CreateLoanProduct();
        dbContext.LoanProducts.Add(loanProduct);

        dbContext.CustomerProfiles.Add(new CustomerProfile(
            customerUserId,
            "Amina Rahman",
            new DateOnly(1995, 5, 12),
            "1234567890123",
            "01712345678",
            "Dhaka",
            "Dhaka"));
        await dbContext.SaveChangesAsync();

        var application = new LoanApplication(
            customerUserId,
            loanProductId: loanProduct.Id,
            applicantFullName: "Amina Rahman",
            requestedAmount: 250000m,
            requestedTenureMonths: 36,
            monthlyIncome: 55000m,
            employmentType: EmploymentType.Salaried,
            loanPurpose: LoanPurpose.Other);

        application.SetEmploymentInformation(
            EmploymentType.Salaried,
            "LoanFlow Ltd",
            "Officer",
            new DateOnly(2022, 1, 10),
            40000m,
            5000m);

        application.SetMonthlyFinances(8000m, 12000m, 3000m, 2000m);

        application.UpdateDraftDetails(
            "Amina Rahman",
            250000m,
            36,
            application.MonthlyIncome,
            EmploymentType.Salaried,
            LoanPurpose.Other,
            "Small business equipment");

        dbContext.LoanApplications.Add(application);
        await dbContext.SaveChangesAsync();
        return application;
    }

    private static LoanProduct CreateLoanProduct()
    {
        return new LoanProduct(
            "Personal Loan",
            "PL-001",
            50000m,
            500000m,
            10.5m,
            12,
            60,
            25000m,
            0.5m,
            true);
    }
}
