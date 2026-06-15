using System.Security.Claims;
using LoanFlow.Application.Authorization;

namespace LoanFlow.Web.Components;

public static class RoleDashboardRoutes
{
    public const string Customer = "/customer/dashboard";
    public const string CustomerProfile = "/customer/profile";
    public const string CustomerApplications = "/customer/applications";
    public const string CustomerNewApplication = "/customer/applications/new";
    public const string OfficerApplications = "/officer/applications";
    public const string LoanOfficer = "/officer/dashboard";
    public const string Approver = "/approver/dashboard";
    public const string Administrator = "/admin/dashboard";

    public static string? GetDashboardPath(ClaimsPrincipal user)
    {
        if (user.IsInRole(AppRoles.Administrator))
        {
            return Administrator;
        }

        if (user.IsInRole(AppRoles.Approver))
        {
            return Approver;
        }

        if (user.IsInRole(AppRoles.LoanOfficer))
        {
            return LoanOfficer;
        }

        if (user.IsInRole(AppRoles.Customer))
        {
            return Customer;
        }

        return null;
    }
}
