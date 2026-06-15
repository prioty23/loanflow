using LoanFlow.Domain.Entities;

namespace LoanFlow.UnitTests;

public class CustomerProfileTests
{
    [Fact]
    public void CustomerProfile_StoresExpectedValues()
    {
        var profile = CreateProfile();

        Assert.Equal("Nusrat Jahan", profile.FullName);
        Assert.Equal(new DateOnly(1998, 3, 14), profile.DateOfBirth);
        Assert.Equal("01712345678", profile.MobileNumber);
        Assert.Equal("1234567890123", profile.NationalIdNumber);
    }

    [Fact]
    public void CustomerProfile_Throws_WhenCustomerIsUnder18()
    {
        var under18BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddYears(-17);

        Assert.Throws<ArgumentOutOfRangeException>(() => new CustomerProfile(
            userId: "user-1",
            fullName: "Nusrat Jahan",
            dateOfBirth: under18BirthDate,
            nationalIdNumber: "1234567890123",
            mobileNumber: "01712345678",
            currentAddress: "Dhaka",
            permanentAddress: "Dhaka"));
    }

    [Fact]
    public void CustomerProfile_Throws_WhenMobileNumberIsInvalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CustomerProfile(
            userId: "user-1",
            fullName: "Nusrat Jahan",
            dateOfBirth: new DateOnly(1998, 3, 14),
            nationalIdNumber: "1234567890123",
            mobileNumber: "12345",
            currentAddress: "Dhaka",
            permanentAddress: "Dhaka"));
    }

    [Fact]
    public void CustomerProfile_Throws_WhenNationalIdContainsNonDigits()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CustomerProfile(
            userId: "user-1",
            fullName: "Nusrat Jahan",
            dateOfBirth: new DateOnly(1998, 3, 14),
            nationalIdNumber: "12345ABC90123",
            mobileNumber: "01712345678",
            currentAddress: "Dhaka",
            permanentAddress: "Dhaka"));
    }

    private static CustomerProfile CreateProfile()
    {
        return new CustomerProfile(
            userId: "user-1",
            fullName: "Nusrat Jahan",
            dateOfBirth: new DateOnly(1998, 3, 14),
            nationalIdNumber: "1234567890123",
            mobileNumber: "01712345678",
            currentAddress: "Road 10, Dhaka",
            permanentAddress: "Road 10, Dhaka");
    }
}
