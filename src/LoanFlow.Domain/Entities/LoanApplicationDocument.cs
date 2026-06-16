using LoanFlow.Domain.Enums;

namespace LoanFlow.Domain.Entities;

public class LoanApplicationDocument
{
    public int Id { get; private set; }

    public int LoanApplicationId { get; private set; }

    public DocumentType DocumentType { get; private set; }

    public VerificationStatus VerificationStatus { get; private set; }

    public string? Note { get; private set; }

    private LoanApplicationDocument()
    {
    }

    public LoanApplicationDocument(DocumentType documentType)
    {
        DocumentType = documentType;
        VerificationStatus = VerificationStatus.Missing;
    }

    public void SetVerificationStatus(VerificationStatus verificationStatus, string? note = null)
    {
        VerificationStatus = verificationStatus;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
