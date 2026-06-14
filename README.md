# LoanFlow

LoanFlow is a modular monolith for loan origination workflows built on .NET 10.

## Solution Structure

- `src/LoanFlow.Domain`: Core business entities and domain rules.
- `src/LoanFlow.Application`: Use cases, contracts, and authorization constants.
- `src/LoanFlow.Infrastructure`: Persistence, identity, EF Core, and seed data.
- `src/LoanFlow.Web`: Blazor web application and startup composition root.
- `tests/LoanFlow.UnitTests`: Automated unit tests.
- `docs/architecture.md`: High-level architecture notes.
- `docs/phase1-demo.md`: Phase 1 setup, demo, and authorization matrix.

## Prerequisites

- .NET 10 SDK
- SQL Server LocalDB, SQL Server Express, or SQL Server Developer
- Git

## First-Time Setup

1. Restore tools and packages:

```powershell
dotnet restore LoanFlow.sln
```

2. Add the development connection string to user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\MSSQLLocalDB;Database=LoanFlow;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True" --project src/LoanFlow.Web
```

3. Add the development-user passwords to user secrets:

```powershell
dotnet user-secrets set "SeedData:DevelopmentUsers:CustomerPassword" "YourStrongPass1" --project src/LoanFlow.Web
dotnet user-secrets set "SeedData:DevelopmentUsers:LoanOfficerPassword" "YourStrongPass1" --project src/LoanFlow.Web
dotnet user-secrets set "SeedData:DevelopmentUsers:ApproverPassword" "YourStrongPass1" --project src/LoanFlow.Web
dotnet user-secrets set "SeedData:DevelopmentUsers:AdministratorPassword" "YourStrongPass1" --project src/LoanFlow.Web
```

4. Apply the database migrations:

```powershell
dotnet ef database update --project src/LoanFlow.Infrastructure --startup-project src/LoanFlow.Web
```

## Run The App

```powershell
dotnet run --project src/LoanFlow.Web
```

On startup the application will:

- apply EF Core migrations
- seed the four roles
- seed four development users
- seed one sample loan product

## Demo Accounts

Passwords are stored in user secrets and are not tracked by Git.

- `customer@loanflow.local`
- `officer@loanflow.local`
- `approver@loanflow.local`
- `admin@loanflow.local`

## Quality Commands

```powershell
dotnet format --verify-no-changes
dotnet build LoanFlow.sln
dotnet test LoanFlow.sln
git status
```

## Checkpoint Status

- `CP-01`: Solution scaffold
- `CP-02`: Architecture baseline
- `CP-03`: SQL Server and EF Core
- `CP-04`: Authentication
- `CP-05`: Roles and development users
- `CP-06`: Protected dashboards
- `CP-07`: Sample loan configuration
- `CP-08`: Final quality gate
