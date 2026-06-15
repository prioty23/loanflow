namespace LoanFlow.Domain.Entities;

public class ApplicantSnapshot
{
    public int Id { get; private set; }

    public int LoanApplicationId { get; private set; }

    public string FullName { get; private set; } = "";

    public DateOnly DateOfBirth { get; private set; }

    public string NationalIdNumber { get; private set; } = "";

    public string MobileNumber { get; private set; } = "";

    public string CurrentAddress { get; private set; } = "";

    public string PermanentAddress { get; private set; } = "";

    private ApplicantSnapshot()
    {
    }

    public ApplicantSnapshot(
        string fullName,
        DateOnly dateOfBirth,
        string nationalIdNumber,
        string mobileNumber,
        string currentAddress,
        string permanentAddress)
    {
        FullName = fullName.Trim();
        DateOfBirth = dateOfBirth;
        NationalIdNumber = nationalIdNumber.Trim();
        MobileNumber = mobileNumber.Trim();
        CurrentAddress = currentAddress.Trim();
        PermanentAddress = permanentAddress.Trim();
    }
}
