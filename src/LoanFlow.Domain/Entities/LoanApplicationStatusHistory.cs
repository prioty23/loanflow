using LoanFlow.Domain.Enums;

namespace LoanFlow.Domain.Entities;

public class LoanApplicationStatusHistory
{
    public int Id { get; private set; }

    public int LoanApplicationId { get; private set; }

    public ApplicationStatus FromStatus { get; private set; }

    public ApplicationStatus ToStatus { get; private set; }

    public DateTime ChangedAtUtc { get; private set; }

    public string? Note { get; private set; }

    private LoanApplicationStatusHistory()
    {
    }

    public LoanApplicationStatusHistory(
        ApplicationStatus fromStatus,
        ApplicationStatus toStatus,
        DateTime changedAtUtc,
        string? note)
    {
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedAtUtc = changedAtUtc;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
