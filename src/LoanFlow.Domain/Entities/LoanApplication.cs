using LoanFlow.Domain.Enums;
using LoanFlow.Domain.Exceptions;

namespace LoanFlow.Domain.Entities;

public class LoanApplication
{
    public const int SubmissionReferenceMaxLength = 32;
    private static readonly DocumentType[] RequiredDocumentTypes =
    [
        DocumentType.NationalId,
        DocumentType.IncomeProof,
        DocumentType.BankStatement
    ];

    public int Id { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public string CustomerUserId { get; private set; } = "";

    public int LoanProductId { get; private set; }

    public string ApplicantFullName { get; private set; } = "";

    public decimal RequestedAmount { get; private set; }

    public int RequestedTenureMonths { get; private set; }

    public decimal MonthlyIncome { get; private set; }

    public EmploymentType EmploymentType { get; private set; }

    public LoanPurpose LoanPurpose { get; private set; }

    public string PurposeDescription { get; private set; } = "";

    public ApplicationStatus Status { get; private set; }

    public LoanApplicationStep LastCompletedStep { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? SubmittedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public bool DeclarationAccepted { get; private set; }

    public string? AssignedLoanOfficerId { get; private set; }

    public string? SubmissionReference { get; private set; }

    public ApplicantSnapshot? ApplicantSnapshot { get; private set; }

    public LoanApplicationFinancialProfile? FinancialProfile { get; private set; }

    public List<LoanApplicationDocument> Documents { get; private set; } = [];

    public List<LoanApplicationStatusHistory> StatusHistory { get; private set; } = [];

    private LoanApplication()
    {
    }

    public LoanApplication(
        string customerUserId,
        int loanProductId,
        string applicantFullName,
        decimal requestedAmount,
        int requestedTenureMonths,
        decimal monthlyIncome,
        EmploymentType employmentType,
        LoanPurpose loanPurpose)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerUserId);

        if (loanProductId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loanProductId));
        }

        CustomerUserId = customerUserId.Trim();
        LoanProductId = loanProductId;
        Status = ApplicationStatus.Draft;
        LastCompletedStep = 0;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;

        UpdateDraftDetails(
            applicantFullName,
            requestedAmount,
            requestedTenureMonths,
            monthlyIncome,
            employmentType,
            loanPurpose);
    }

    public void UpdateDraftDetails(
        string applicantFullName,
        decimal requestedAmount,
        int requestedTenureMonths,
        decimal monthlyIncome,
        EmploymentType employmentType,
        LoanPurpose loanPurpose,
        string purposeDescription = "")
    {
        EnsureDraft();

        ArgumentException.ThrowIfNullOrWhiteSpace(applicantFullName);

        if (requestedAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedAmount));
        }

        if (requestedTenureMonths <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedTenureMonths));
        }

        if (monthlyIncome < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monthlyIncome));
        }

        ApplicantFullName = applicantFullName.Trim();
        RequestedAmount = requestedAmount;
        RequestedTenureMonths = requestedTenureMonths;
        MonthlyIncome = monthlyIncome;
        EmploymentType = employmentType;
        LoanPurpose = loanPurpose;
        PurposeDescription = purposeDescription?.Trim() ?? "";
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Submit(string submissionReference, ApplicantSnapshot applicantSnapshot, DateTime submittedAtUtc)
    {
        if (!DeclarationAccepted)
        {
            throw new DomainRuleException("You must accept the declaration before submitting.");
        }

        if (Status == ApplicationStatus.Submitted || SubmittedAtUtc is not null || !string.IsNullOrWhiteSpace(SubmissionReference))
        {
            throw new DomainRuleException("This application has already been submitted.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(submissionReference);

        if (submissionReference.Trim().Length > SubmissionReferenceMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(submissionReference));
        }

        ArgumentNullException.ThrowIfNull(applicantSnapshot);

        SubmissionReference = submissionReference.Trim();
        ApplicantSnapshot = applicantSnapshot;
        SubmittedAtUtc = submittedAtUtc;
        ChangeStatus(ApplicationStatus.Submitted, submittedAtUtc, "Application submitted.");
    }

    public void MarkStepCompleted(LoanApplicationStep step)
    {
        if (step < LoanApplicationStep.Employment || step > LoanApplicationStep.ReviewAndDeclaration)
        {
            throw new ArgumentOutOfRangeException(nameof(step));
        }

        if (step > LastCompletedStep)
        {
            LastCompletedStep = step;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    public void SetDeclarationAccepted(bool declarationAccepted)
    {
        EnsureDraft();
        DeclarationAccepted = declarationAccepted;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ReturnToDraft()
    {
        throw new DomainRuleException("Submitted applications cannot return to draft.");
    }

    public void SetEmploymentInformation(
        EmploymentType employmentType,
        string employerOrBusinessName,
        string jobTitle,
        DateOnly employmentStartDate,
        decimal monthlyNetSalary,
        decimal otherMonthlyIncome)
    {
        EnsureDraft();

        if (FinancialProfile is null)
        {
            FinancialProfile = new LoanApplicationFinancialProfile(
                employmentType,
                employerOrBusinessName,
                jobTitle,
                employmentStartDate,
                monthlyNetSalary,
                otherMonthlyIncome);
        }
        else
        {
            FinancialProfile.UpdateEmployment(
                employmentType,
                employerOrBusinessName,
                jobTitle,
                employmentStartDate,
                monthlyNetSalary,
                otherMonthlyIncome);
        }

        EmploymentType = employmentType;
        MonthlyIncome = FinancialProfile.TotalMonthlyIncome;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetMonthlyFinances(
        decimal housingExpense,
        decimal livingExpense,
        decimal existingMonthlyEmi,
        decimal otherLiabilities)
    {
        EnsureDraft();

        if (FinancialProfile is null)
        {
            throw new DomainRuleException("Employment information must be saved before monthly finances.");
        }

        FinancialProfile.UpdateMonthlyFinances(
            housingExpense,
            livingExpense,
            existingMonthlyEmi,
            otherLiabilities);

        MonthlyIncome = FinancialProfile.TotalMonthlyIncome;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private void EnsureDraft()
    {
        if (Status != ApplicationStatus.Draft)
        {
            throw new DomainRuleException("Only draft applications can be changed.");
        }
    }

    public void StartReviewByLoanOfficer(string loanOfficerId, DateTime changedAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loanOfficerId);

        var normalizedLoanOfficerId = loanOfficerId.Trim();
        if (normalizedLoanOfficerId == CustomerUserId)
        {
            throw new DomainRuleException("The customer cannot start review for their own application.");
        }

        EnsureCurrentStatus(ApplicationStatus.Submitted);
        AssignedLoanOfficerId = normalizedLoanOfficerId;
        ChangeStatus(ApplicationStatus.UnderReview, changedAtUtc, "Review started by loan officer.");
    }

    public void RequestInformation(string explanation, DateTime changedAtUtc)
    {
        EnsureCurrentStatus(ApplicationStatus.UnderReview);

        if (string.IsNullOrWhiteSpace(explanation))
        {
            throw new DomainRuleException("An explanation is required when requesting more information.");
        }

        ChangeStatus(
            ApplicationStatus.InformationRequested,
            changedAtUtc,
            explanation.Trim());
    }

    public void ResubmitRequestedInformation(DateTime changedAtUtc)
    {
        EnsureCurrentStatus(ApplicationStatus.InformationRequested);
        ChangeStatus(ApplicationStatus.Submitted, changedAtUtc, "Customer resubmitted the requested information.");
    }

    public void MarkReadyForAssessment(DateTime changedAtUtc)
    {
        EnsureCurrentStatus(ApplicationStatus.UnderReview);

        if (!AreAllRequiredDocumentsVerified())
        {
            throw new DomainRuleException(
                "All required documents must be verified before the application can become ReadyForAssessment.");
        }

        ChangeStatus(ApplicationStatus.ReadyForAssessment, changedAtUtc, "Application is ready for assessment.");
    }

    public void SetDocumentVerification(
        DocumentType documentType,
        VerificationStatus verificationStatus,
        string? note = null)
    {
        var document = Documents.SingleOrDefault(existingDocument => existingDocument.DocumentType == documentType);

        if (document is null)
        {
            document = new LoanApplicationDocument(documentType);
            Documents.Add(document);
        }

        document.SetVerificationStatus(verificationStatus, note);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private bool AreAllRequiredDocumentsVerified()
    {
        return RequiredDocumentTypes.All(requiredDocumentType =>
            Documents.Any(document =>
                document.DocumentType == requiredDocumentType &&
                document.VerificationStatus == VerificationStatus.Verified));
    }

    private void EnsureCurrentStatus(ApplicationStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new DomainRuleException(
                $"The status cannot change from {Status} using this action.");
        }
    }

    private void ChangeStatus(ApplicationStatus nextStatus, DateTime changedAtUtc, string note)
    {
        var previousStatus = Status;
        Status = nextStatus;
        UpdatedAtUtc = changedAtUtc;

        StatusHistory.Add(new LoanApplicationStatusHistory(
            fromStatus: previousStatus,
            toStatus: nextStatus,
            changedAtUtc: changedAtUtc,
            note: note));
    }
}
