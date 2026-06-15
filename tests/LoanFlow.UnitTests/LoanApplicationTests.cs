using LoanFlow.Application.CustomerProfiles;
using LoanFlow.Application.LoanApplications;
using LoanFlow.Application.Validation;
using LoanFlow.Domain.Entities;
using LoanFlow.Domain.Enums;

namespace LoanFlow.UnitTests;

public class LoanApplicationTests
{
    [Fact]
    public void NewApplication_StartsAsDraft()
    {
        var application = CreateApplication();

        Assert.Equal("customer-1", application.CustomerUserId);
        Assert.Equal(1, application.LoanProductId);
        Assert.Equal(ApplicationStatus.Draft, application.Status);
        Assert.Equal(0, (int)application.LastCompletedStep);
        Assert.NotEqual(default, application.CreatedAtUtc);
        Assert.Null(application.SubmittedAtUtc);
    }

    [Fact]
    public void Submit_ChangesDraftToSubmitted()
    {
        var application = CreateApplication();
        application.SetDeclarationAccepted(true);

        application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc());

        Assert.Equal(ApplicationStatus.Submitted, application.Status);
        Assert.NotNull(application.SubmittedAtUtc);
        Assert.NotNull(application.SubmissionReference);
        Assert.NotNull(application.ApplicantSnapshot);
        Assert.Single(application.StatusHistory);
    }

    [Fact]
    public void DraftApplication_CanBeUpdated()
    {
        var application = CreateApplication();

        application.UpdateDraftDetails(
            applicantFullName: "  Amina Updated  ",
            requestedAmount: 300000m,
            requestedTenureMonths: 48,
            monthlyIncome: 60000m,
            employmentType: EmploymentType.SelfEmployed,
            loanPurpose: LoanPurpose.HomeImprovement);

        Assert.Equal("Amina Updated", application.ApplicantFullName);
        Assert.Equal(300000m, application.RequestedAmount);
        Assert.Equal(48, application.RequestedTenureMonths);
        Assert.Equal(60000m, application.MonthlyIncome);
        Assert.Equal(EmploymentType.SelfEmployed, application.EmploymentType);
        Assert.Equal(LoanPurpose.HomeImprovement, application.LoanPurpose);
        Assert.Equal(ApplicationStatus.Draft, application.Status);
    }

    [Fact]
    public void SubmittedApplication_CannotReturnToDraft()
    {
        var application = CreateApplication();
        application.SetDeclarationAccepted(true);
        application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc());

        var exception = Assert.Throws<InvalidOperationException>(() => application.ReturnToDraft());

        Assert.Equal("Submitted applications cannot return to draft.", exception.Message);
        Assert.Equal(ApplicationStatus.Submitted, application.Status);
    }

    [Fact]
    public void SubmittedApplication_CannotBeSubmittedAgain()
    {
        var application = CreateApplication();
        application.SetDeclarationAccepted(true);
        application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc());

        var exception = Assert.Throws<InvalidOperationException>(() =>
            application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc()));

        Assert.Equal("This application has already been submitted.", exception.Message);
    }

    [Fact]
    public void SubmittedApplication_CannotBeEditedSilently()
    {
        var application = CreateApplication();
        application.SetDeclarationAccepted(true);
        application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc());

        Assert.Throws<InvalidOperationException>(() => application.UpdateDraftDetails(
            applicantFullName: "Changed Name",
            requestedAmount: 300000m,
            requestedTenureMonths: 48,
            monthlyIncome: 60000m,
            employmentType: EmploymentType.SelfEmployed,
            loanPurpose: LoanPurpose.HomeImprovement));
    }

    [Fact]
    public void SubmittedApplication_CannotChangeDeclaration()
    {
        var application = CreateApplication();
        application.SetDeclarationAccepted(true);
        application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc());

        Assert.Throws<InvalidOperationException>(() => application.SetDeclarationAccepted(false));
    }

    [Fact]
    public void MarkStepCompleted_StoresTheLatestCompletedStep()
    {
        var application = CreateApplication();

        application.MarkStepCompleted(LoanApplicationStep.Employment);
        application.MarkStepCompleted(LoanApplicationStep.MonthlyFinances);

        Assert.Equal(LoanApplicationStep.MonthlyFinances, application.LastCompletedStep);
    }

    [Fact]
    public void Submit_Throws_WhenDeclarationWasNotAccepted()
    {
        var application = CreateApplication();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc()));

        Assert.Equal("You must accept the declaration before submitting.", exception.Message);
    }

    [Fact]
    public void Submit_CopiesApplicantSnapshot()
    {
        var application = CreateApplication();
        application.SetDeclarationAccepted(true);

        application.Submit(CreateReference(), CreateApplicantSnapshot(), CreateSubmittedAtUtc());

        Assert.NotNull(application.ApplicantSnapshot);
        Assert.Equal("Amina Rahman", application.ApplicantSnapshot!.FullName);
        Assert.Equal("1234567890123", application.ApplicantSnapshot.NationalIdNumber);
    }

    [Fact]
    public void EmploymentInformation_CalculatesTotalMonthlyIncome()
    {
        var application = CreateApplication();

        application.SetEmploymentInformation(
            EmploymentType.Salaried,
            "LoanFlow Ltd",
            "Officer",
            new DateOnly(2022, 1, 10),
            40000m,
            5000m);

        Assert.Equal(45000m, application.MonthlyIncome);
        Assert.NotNull(application.FinancialProfile);
        Assert.Equal(45000m, application.FinancialProfile!.TotalMonthlyIncome);
    }

    [Fact]
    public void MonthlyFinances_CalculatesExpensesAndDisposableIncome()
    {
        var application = CreateApplication();
        application.SetEmploymentInformation(
            EmploymentType.Salaried,
            "LoanFlow Ltd",
            "Officer",
            new DateOnly(2022, 1, 10),
            40000m,
            5000m);

        application.SetMonthlyFinances(8000m, 12000m, 3000m, 2000m);

        Assert.Equal(25000m, application.FinancialProfile!.TotalMonthlyExpenses);
        Assert.Equal(20000m, application.FinancialProfile.CalculatedDisposableIncome);
    }

    [Fact]
    public void EmploymentInformation_RejectsFutureStartDate()
    {
        var application = CreateApplication();

        var exception = Assert.Throws<ArgumentException>(() => application.SetEmploymentInformation(
            EmploymentType.Salaried,
            "LoanFlow Ltd",
            "Officer",
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            40000m,
            5000m));

        Assert.Equal("employmentStartDate", exception.ParamName);
    }

    [Fact]
    public void DraftEmployment_AllowsMissingEmployerForSalariedApplicant()
    {
        var application = CreateApplication();

        application.SetEmploymentInformation(
            EmploymentType.Salaried,
            "",
            "Officer",
            new DateOnly(2022, 1, 10),
            40000m,
            5000m);

        Assert.NotNull(application.FinancialProfile);
        Assert.Equal(string.Empty, application.FinancialProfile!.EmployerOrBusinessName);
    }

    [Fact]
    public void MonthlyFinances_RejectNegativeValues()
    {
        var application = CreateApplication();
        application.SetEmploymentInformation(
            EmploymentType.Salaried,
            "LoanFlow Ltd",
            "Officer",
            new DateOnly(2022, 1, 10),
            40000m,
            5000m);

        Assert.Throws<ArgumentOutOfRangeException>(() => application.SetMonthlyFinances(-1m, 0m, 0m, 0m));
    }

    [Fact]
    public void DraftApplication_AllowsMissingPurposeDescriptionForOther()
    {
        var application = CreateApplication();

        application.UpdateDraftDetails(
            applicantFullName: "Amina Rahman",
            requestedAmount: 250000m,
            requestedTenureMonths: 36,
            monthlyIncome: 55000m,
            employmentType: EmploymentType.Salaried,
            loanPurpose: LoanPurpose.Other,
            purposeDescription: "");

        Assert.Equal(LoanPurpose.Other, application.LoanPurpose);
        Assert.Equal(string.Empty, application.PurposeDescription);
    }

    [Fact]
    public void SubmitValidator_RejectsIncompleteDraft()
    {
        var validator = new SubmitLoanApplicationCommandValidator();
        var command = new SubmitLoanApplicationCommand
        {
            CustomerProfile = null,
            EmploymentDetails = new EmploymentDetailsDto(),
            FinancialDetails = new FinancialDetailsDto(),
            LoanRequest = new LoanRequestDto
            {
                LoanPurpose = LoanPurpose.Other
            },
            DeclarationAccepted = false,
            ProductIsActive = true,
            ProductMinimumLoanAmount = 50000m,
            ProductMaximumLoanAmount = 500000m,
            ProductMinimumTenureMonths = 12,
            ProductMaximumTenureMonths = 60
        };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "CustomerProfile");
        Assert.Contains(result.Errors, error => error.PropertyName == "EmploymentDetails.JobTitle");
        Assert.Contains(result.Errors, error => error.PropertyName == "LoanRequest.ApplicantFullName");
        Assert.Contains(result.Errors, error => error.PropertyName == "LoanRequest.PurposeDescription");
        Assert.Contains(result.Errors, error => error.PropertyName == "DeclarationAccepted");
    }

    [Fact]
    public void SubmitValidator_AcceptsCompleteApplication()
    {
        var validator = new SubmitLoanApplicationCommandValidator();
        var command = new SubmitLoanApplicationCommand
        {
            CustomerProfile = new CustomerProfileDto
            {
                FullName = "Amina Rahman",
                DateOfBirth = new DateOnly(1995, 5, 12),
                NationalIdNumber = "1234567890123",
                MobileNumber = "01712345678",
                CurrentAddress = "Dhaka",
                PermanentAddress = "Dhaka",
                IsPermanentAddressSameAsPresentAddress = true
            },
            EmploymentDetails = new EmploymentDetailsDto
            {
                EmploymentType = EmploymentType.Salaried,
                EmployerOrBusinessName = "LoanFlow Ltd",
                JobTitle = "Officer",
                EmploymentStartDate = new DateOnly(2022, 1, 10),
                MonthlyNetSalary = 40000m,
                OtherMonthlyIncome = 5000m
            },
            FinancialDetails = new FinancialDetailsDto
            {
                HousingExpense = 8000m,
                LivingExpense = 12000m,
                ExistingMonthlyEmi = 3000m,
                OtherLiabilities = 2000m
            },
            LoanRequest = new LoanRequestDto
            {
                ApplicantFullName = "Amina Rahman",
                RequestedAmount = 250000m,
                RequestedTenureMonths = 36,
                LoanPurpose = LoanPurpose.Other,
                PurposeDescription = "Small business equipment"
            },
            DeclarationAccepted = true,
            ProductIsActive = true,
            ProductMinimumLoanAmount = 50000m,
            ProductMaximumLoanAmount = 500000m,
            ProductMinimumTenureMonths = 12,
            ProductMaximumTenureMonths = 60
        };

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void LoanProduct_RejectsInactiveProduct()
    {
        var product = CreateLoanProduct(isActive: false);

        var exception = Assert.Throws<InvalidOperationException>(() => product.ValidateLoanRequest(250000m, 36));

        Assert.Equal("The selected loan product is no longer active.", exception.Message);
    }

    [Fact]
    public void LoanProduct_RejectsOutOfRangeAmount()
    {
        var product = CreateLoanProduct();

        var exception = Assert.Throws<InvalidOperationException>(() => product.ValidateLoanRequest(999999m, 36));

        Assert.Equal("Requested amount must be between 50000 and 500000.", exception.Message);
    }

    [Fact]
    public void LoanProduct_RejectsOutOfRangeTenure()
    {
        var product = CreateLoanProduct();

        var exception = Assert.Throws<InvalidOperationException>(() => product.ValidateLoanRequest(250000m, 6));

        Assert.Equal("Requested tenure must be between 12 and 60 months.", exception.Message);
    }

    private static LoanApplication CreateApplication()
    {
        return new LoanApplication(
            customerUserId: "customer-1",
            loanProductId: 1,
            applicantFullName: "Amina Rahman",
            requestedAmount: 250000m,
            requestedTenureMonths: 36,
            monthlyIncome: 55000m,
            employmentType: EmploymentType.Salaried,
            loanPurpose: LoanPurpose.Education);
    }

    private static ApplicantSnapshot CreateApplicantSnapshot()
    {
        return new ApplicantSnapshot(
            "Amina Rahman",
            new DateOnly(1995, 5, 12),
            "1234567890123",
            "01712345678",
            "Dhaka",
            "Dhaka");
    }

    private static DateTime CreateSubmittedAtUtc()
    {
        return new DateTime(2026, 6, 15, 9, 30, 0, DateTimeKind.Utc);
    }

    private static string CreateReference()
    {
        return "LF-20260615-A1B2C3D4";
    }

    private static LoanProduct CreateLoanProduct(bool isActive = true)
    {
        return new LoanProduct(
            productName: "Personal Loan",
            productCode: "PL-001",
            minimumLoanAmount: 50000m,
            maximumLoanAmount: 500000m,
            annualInterestRate: 10.5m,
            minimumTenureMonths: 12,
            maximumTenureMonths: 60,
            minimumMonthlyIncome: 25000m,
            maximumDebtToIncomeRatio: 0.5m,
            isActive: isActive);
    }
}
