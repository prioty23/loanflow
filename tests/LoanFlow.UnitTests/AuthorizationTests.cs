using LoanFlow.Application.Authorization;
using LoanFlow.Web.Components.Pages;
using Microsoft.AspNetCore.Authorization;

namespace LoanFlow.UnitTests;

public class AuthorizationTests
{
    [Fact]
    public void OfficerQueuePage_RequiresLoanOfficerRole()
    {
        var attribute = typeof(OfficerApplications)
            .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .Cast<AuthorizeAttribute>()
            .Single();

        Assert.Equal(AppRoles.LoanOfficer, attribute.Roles);
    }
}
