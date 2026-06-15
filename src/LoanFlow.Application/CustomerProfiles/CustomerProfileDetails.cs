namespace LoanFlow.Application.CustomerProfiles;

public sealed record CustomerProfileDetails(
    string FullName,
    DateOnly? DateOfBirth,
    string NationalIdNumber,
    string MobileNumber,
    string CurrentAddress,
    string PermanentAddress);
