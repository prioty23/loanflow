# Phase 1 Demo Guide

## Installation And Setup

1. Install the .NET 10 SDK and SQL Server LocalDB or another SQL Server edition.
2. Restore the solution:

```powershell
dotnet restore LoanFlow.sln
```

3. Store the connection string in user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=LoanFlow;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True" --project src/LoanFlow.Web
```

4. Store the four development-user passwords in user secrets:

```powershell
dotnet user-secrets set "SeedData:DevelopmentUsers:CustomerPassword" "YourStrongPass1" --project src/LoanFlow.Web
dotnet user-secrets set "SeedData:DevelopmentUsers:LoanOfficerPassword" "YourStrongPass1" --project src/LoanFlow.Web
dotnet user-secrets set "SeedData:DevelopmentUsers:ApproverPassword" "YourStrongPass1" --project src/LoanFlow.Web
dotnet user-secrets set "SeedData:DevelopmentUsers:AdministratorPassword" "YourStrongPass1" --project src/LoanFlow.Web
```

5. Update the database:

```powershell
dotnet ef database update --project src/LoanFlow.Infrastructure --startup-project src/LoanFlow.Web
```

6. Run the web app:

```powershell
dotnet run --project src/LoanFlow.Web
```

## Demo Accounts

Use the passwords from user secrets with these accounts:

- `customer@loanflow.local`
- `officer@loanflow.local`
- `approver@loanflow.local`
- `admin@loanflow.local`

## Sample Loan Product

The administrator dashboard displays one seeded read-only product:

- Product name: `Everyday Personal Loan`
- Product code: `PL-START`

## Authorization Test Matrix

| Signed-In User | `/customer/dashboard` | `/officer/dashboard` | `/approver/dashboard` | `/admin/dashboard` |
| --- | --- | --- | --- | --- |
| Customer | Allow | Deny | Deny | Deny |
| Loan Officer | Deny | Allow | Deny | Deny |
| Approver | Deny | Deny | Allow | Deny |
| Administrator | Deny | Deny | Deny | Allow |

## Manual Demo Flow

1. Sign in as each development user.
2. Confirm the home route `/` redirects to that user’s own dashboard.
3. Confirm the navigation shows only the correct dashboard link for that role.
4. Try opening all four dashboard routes and compare the results with the table above.
5. Sign in as `admin@loanflow.local` and open `/admin/dashboard`.
6. Confirm the sample loan product appears in the read-only section.
7. Restart the application and confirm the same product still appears once.

## Secret Safety Notes

- Development passwords are stored in user secrets, not in source control.
- The database connection string is stored in user secrets, not in `appsettings.json`.
- Local SQL Server files like `*.mdf` and `*.ldf` are ignored by Git.
