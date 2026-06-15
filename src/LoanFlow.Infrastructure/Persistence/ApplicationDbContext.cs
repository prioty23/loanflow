using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ApplicantSnapshot> ApplicantSnapshots => Set<ApplicantSnapshot>();

    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();

    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    public DbSet<LoanApplicationFinancialProfile> LoanApplicationFinancialProfiles => Set<LoanApplicationFinancialProfile>();

    public DbSet<LoanApplicationStatusHistory> LoanApplicationStatusHistory => Set<LoanApplicationStatusHistory>();

    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
