namespace LoanFlow.Application.CustomerProfiles;

public sealed class CustomerProfileDto
{
    public string FullName { get; set; } = "";

    public DateOnly? DateOfBirth { get; set; }

    public string NationalIdNumber { get; set; } = "";

    public string MobileNumber { get; set; } = "";

    public string CurrentAddress { get; set; } = "";

    public string PermanentAddress { get; set; } = "";

    public bool IsPermanentAddressSameAsPresentAddress { get; set; }
}
