using LoanFlow.Application.CustomerProfiles;
using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Persistence;

public sealed class CustomerProfileService(ApplicationDbContext dbContext) : ICustomerProfileService
{
    public async Task<CustomerProfileDetails?> GetForUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CustomerProfiles
            .AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .Select(profile => new CustomerProfileDetails(
                profile.FullName,
                profile.DateOfBirth,
                profile.NationalIdNumber,
                profile.MobileNumber,
                profile.CurrentAddress,
                profile.PermanentAddress))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task SaveForUserAsync(
        string userId,
        SaveCustomerProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.CustomerProfiles
            .SingleOrDefaultAsync(existingProfile => existingProfile.UserId == userId, cancellationToken);

        if (profile is null)
        {
            profile = new CustomerProfile(
                userId,
                request.FullName,
                request.DateOfBirth,
                request.NationalIdNumber,
                request.MobileNumber,
                request.CurrentAddress,
                request.PermanentAddress);

            dbContext.CustomerProfiles.Add(profile);
        }
        else
        {
            profile.Update(
                userId,
                request.FullName,
                request.DateOfBirth,
                request.NationalIdNumber,
                request.MobileNumber,
                request.CurrentAddress,
                request.PermanentAddress);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
