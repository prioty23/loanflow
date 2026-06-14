namespace LoanFlow.Application.Authorization;

public static class AppRoles
{
    public const string Customer = nameof(Customer);
    public const string LoanOfficer = nameof(LoanOfficer);
    public const string Approver = nameof(Approver);
    public const string Administrator = nameof(Administrator);

    public static readonly string[] All =
    [
        Customer,
        LoanOfficer,
        Approver,
        Administrator
    ];
}
