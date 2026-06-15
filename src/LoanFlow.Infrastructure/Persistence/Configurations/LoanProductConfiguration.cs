using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

public class LoanProductConfiguration : IEntityTypeConfiguration<LoanProduct>
{
    public void Configure(EntityTypeBuilder<LoanProduct> builder)
    {
        builder.HasKey(product => product.Id);

        builder.HasIndex(product => product.ProductCode)
            .IsUnique();

        builder.Property(product => product.ProductName)
            .HasMaxLength(120);

        builder.Property(product => product.ProductCode)
            .HasMaxLength(30);

        builder.Property(product => product.MinimumLoanAmount)
            .HasPrecision(18, 2);

        builder.Property(product => product.MaximumLoanAmount)
            .HasPrecision(18, 2);

        builder.Property(product => product.AnnualInterestRate)
            .HasPrecision(5, 2);

        builder.Property(product => product.MinimumMonthlyIncome)
            .HasPrecision(18, 2);

        builder.Property(product => product.MaximumDebtToIncomeRatio)
            .HasPrecision(4, 2);
    }
}
