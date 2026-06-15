namespace LoanFlow.Domain.Entities;

public class CustomerProfile
{
    public const int FullNameMaxLength = 150;
    public const int NationalIdMinLength = 10;
    public const int NationalIdMaxLength = 17;
    public const int MobileNumberLength = 11;
    public const int AddressMaxLength = 300;

    public int Id { get; private set; }

    public string UserId { get; private set; } = "";

    public byte[] RowVersion { get; private set; } = [];

    public string FullName { get; private set; } = "";

    public DateOnly DateOfBirth { get; private set; }

    public string NationalIdNumber { get; private set; } = "";

    public string MobileNumber { get; private set; } = "";

    public string CurrentAddress { get; private set; } = "";

    public string PermanentAddress { get; private set; } = "";

    private CustomerProfile()
    {
    }

    public CustomerProfile(
        string userId,
        string fullName,
        DateOnly dateOfBirth,
        string nationalIdNumber,
        string mobileNumber,
        string currentAddress,
        string permanentAddress)
    {
        Update(
            userId,
            fullName,
            dateOfBirth,
            nationalIdNumber,
            mobileNumber,
            currentAddress,
            permanentAddress);
    }

    public void Update(
        string userId,
        string fullName,
        DateOnly dateOfBirth,
        string nationalIdNumber,
        string mobileNumber,
        string currentAddress,
        string permanentAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(nationalIdNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(mobileNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(currentAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(permanentAddress);

        var trimmedFullName = fullName.Trim();
        var trimmedNationalIdNumber = nationalIdNumber.Trim();
        var trimmedMobileNumber = mobileNumber.Trim();
        var trimmedCurrentAddress = currentAddress.Trim();
        var trimmedPermanentAddress = permanentAddress.Trim();

        if (trimmedFullName.Length > FullNameMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(fullName));
        }

        if (!IsAtLeast18(dateOfBirth))
        {
            throw new ArgumentOutOfRangeException(nameof(dateOfBirth), "Customer must be at least 18 years old.");
        }

        if (!trimmedNationalIdNumber.All(char.IsDigit) ||
            trimmedNationalIdNumber.Length < NationalIdMinLength ||
            trimmedNationalIdNumber.Length > NationalIdMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(nationalIdNumber));
        }

        if (!IsValidBangladeshMobileNumber(trimmedMobileNumber))
        {
            throw new ArgumentOutOfRangeException(nameof(mobileNumber));
        }

        if (trimmedCurrentAddress.Length > AddressMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(currentAddress));
        }

        if (trimmedPermanentAddress.Length > AddressMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(permanentAddress));
        }

        UserId = userId.Trim();
        FullName = trimmedFullName;
        DateOfBirth = dateOfBirth;
        NationalIdNumber = trimmedNationalIdNumber;
        MobileNumber = trimmedMobileNumber;
        CurrentAddress = trimmedCurrentAddress;
        PermanentAddress = trimmedPermanentAddress;
    }

    private static bool IsAtLeast18(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age >= 18;
    }

    private static bool IsValidBangladeshMobileNumber(string mobileNumber)
    {
        return mobileNumber.Length == MobileNumberLength &&
               mobileNumber.StartsWith("01", StringComparison.Ordinal) &&
               mobileNumber.All(char.IsDigit);
    }
}
