namespace LoanFlow.Application.CustomerProfiles;

public interface ICustomerProfileService
{
    Task<CustomerProfileDetails?> GetForUserAsync(string userId, CancellationToken cancellationToken = default);

    Task SaveForUserAsync(
        string userId,
        SaveCustomerProfileRequest request,
        CancellationToken cancellationToken = default);
}
