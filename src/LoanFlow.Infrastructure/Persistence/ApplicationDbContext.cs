using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoanFlow.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LoanProduct>(entity =>
        {
            entity.HasIndex(product => product.ProductCode)
                .IsUnique();

            entity.Property(product => product.ProductName)
                .HasMaxLength(120);

            entity.Property(product => product.ProductCode)
                .HasMaxLength(30);

            entity.Property(product => product.MinimumLoanAmount)
                .HasPrecision(18, 2);

            entity.Property(product => product.MaximumLoanAmount)
                .HasPrecision(18, 2);

            entity.Property(product => product.AnnualInterestRate)
                .HasPrecision(5, 2);

            entity.Property(product => product.MinimumMonthlyIncome)
                .HasPrecision(18, 2);

            entity.Property(product => product.MaximumDebtToIncomeRatio)
                .HasPrecision(4, 2);
        });
    }
}
