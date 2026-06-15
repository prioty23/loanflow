using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

public class LoanApplicationFinancialProfileConfiguration : IEntityTypeConfiguration<LoanApplicationFinancialProfile>
{
    public void Configure(EntityTypeBuilder<LoanApplicationFinancialProfile> builder)
    {
        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.EmployerOrBusinessName)
            .HasMaxLength(150);

        builder.Property(profile => profile.JobTitle)
            .HasMaxLength(100);

        builder.Property(profile => profile.MonthlyNetSalary)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.OtherMonthlyIncome)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.HousingExpense)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.LivingExpense)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.ExistingMonthlyEmi)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.OtherLiabilities)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.TotalMonthlyIncome)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.TotalMonthlyExpenses)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.CalculatedDisposableIncome)
            .HasPrecision(18, 2);

        builder.Property(profile => profile.RowVersion)
            .IsRowVersion();

        builder.HasIndex(profile => profile.LoanApplicationId)
            .IsUnique();
    }
}
