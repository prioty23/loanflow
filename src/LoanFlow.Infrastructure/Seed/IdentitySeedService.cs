using LoanFlow.Application.Authorization;
using LoanFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LoanFlow.Infrastructure.Seed;

public static class IdentitySeedService
{
    private static readonly DevelopmentUserSeed[] DevelopmentUsers =
    [
        new("customer@loanflow.local", AppRoles.Customer, "SeedData:DevelopmentUsers:CustomerPassword"),
        new("officer@loanflow.local", AppRoles.LoanOfficer, "SeedData:DevelopmentUsers:LoanOfficerPassword"),
        new("approver@loanflow.local", AppRoles.Approver, "SeedData:DevelopmentUsers:ApproverPassword"),
        new("admin@loanflow.local", AppRoles.Administrator, "SeedData:DevelopmentUsers:AdministratorPassword")
    ];

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in AppRoles.All)
        {
            await SeedRoleAsync(roleManager, roleName, cancellationToken);
        }

        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var missingSecrets = DevelopmentUsers
            .Where(user => string.IsNullOrWhiteSpace(configuration[user.PasswordSecretKey]))
            .Select(user => user.PasswordSecretKey)
            .ToArray();

        if (missingSecrets.Length > 0)
        {
            var commands = string.Join(
                Environment.NewLine,
                missingSecrets.Select(secretKey =>
                    $"dotnet user-secrets set \"{secretKey}\" \"<strong-password>\" --project src/LoanFlow.Web"));

            throw new InvalidOperationException(
                "Missing development user passwords in user secrets." + Environment.NewLine +
                "Add the missing secrets and restart the application:" + Environment.NewLine +
                commands);
        }

        foreach (var developmentUser in DevelopmentUsers)
        {
            await SeedDevelopmentUserAsync(
                userManager,
                developmentUser,
                configuration[developmentUser.PasswordSecretKey]!,
                cancellationToken);
        }
    }

    private static async Task SeedRoleAsync(
        RoleManager<IdentityRole> roleManager,
        string roleName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await roleManager.CreateAsync(new IdentityRole(roleName));
        EnsureSucceeded(result, $"Unable to seed role '{roleName}'");
    }

    private static async Task SeedDevelopmentUserAsync(
        UserManager<ApplicationUser> userManager,
        DevelopmentUserSeed developmentUser,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(developmentUser.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = developmentUser.Email,
                Email = developmentUser.Email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, $"Unable to create development user '{developmentUser.Email}'");
        }
        else
        {
            var userChanged = false;

            if (user.UserName != developmentUser.Email)
            {
                user.UserName = developmentUser.Email;
                userChanged = true;
            }

            if (user.Email != developmentUser.Email)
            {
                user.Email = developmentUser.Email;
                userChanged = true;
            }

            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                userChanged = true;
            }

            if (userChanged)
            {
                var updateResult = await userManager.UpdateAsync(user);
                EnsureSucceeded(updateResult, $"Unable to update development user '{developmentUser.Email}'");
            }
        }

        await EnsurePasswordAsync(userManager, user, password);
        await EnsureOnlyRoleAsync(userManager, user, developmentUser.Role);
    }

    private static async Task EnsurePasswordAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string password)
    {
        if (!await userManager.HasPasswordAsync(user))
        {
            var addPasswordResult = await userManager.AddPasswordAsync(user, password);
            EnsureSucceeded(addPasswordResult, $"Unable to add a password for '{user.Email}'");
            return;
        }

        if (await userManager.CheckPasswordAsync(user, password))
        {
            return;
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult = await userManager.ResetPasswordAsync(user, resetToken, password);
        EnsureSucceeded(resetPasswordResult, $"Unable to refresh the password for '{user.Email}'");
    }

    private static async Task EnsureOnlyRoleAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string requiredRole)
    {
        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles
            .Where(role => role != requiredRole)
            .ToArray();

        if (rolesToRemove.Length > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
            EnsureSucceeded(removeResult, $"Unable to remove old roles for '{user.Email}'");
        }

        if (currentRoles.Contains(requiredRole))
        {
            return;
        }

        var addToRoleResult = await userManager.AddToRoleAsync(user, requiredRole);
        EnsureSucceeded(addToRoleResult, $"Unable to assign role '{requiredRole}' to '{user.Email}'");
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(", ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"{message}: {errors}");
    }

    private sealed record DevelopmentUserSeed(string Email, string Role, string PasswordSecretKey);
}
