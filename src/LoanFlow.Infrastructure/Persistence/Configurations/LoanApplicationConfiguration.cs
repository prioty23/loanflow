using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.HasKey(application => application.Id);

        builder.Property(application => application.CustomerUserId)
            .HasMaxLength(450);

        builder.Property(application => application.CreatedAtUtc);

        builder.Property(application => application.UpdatedAtUtc);

        builder.Property(application => application.ApplicantFullName)
            .HasMaxLength(150);

        builder.Property(application => application.PurposeDescription)
            .HasMaxLength(250);

        builder.Property(application => application.SubmissionReference)
            .HasMaxLength(LoanApplication.SubmissionReferenceMaxLength);

        builder.Property(application => application.RequestedAmount)
            .HasPrecision(18, 2);

        builder.Property(application => application.MonthlyIncome)
            .HasPrecision(18, 2);

        builder.Property(application => application.RowVersion)
            .IsRowVersion();

        builder.HasIndex(application => application.CustomerUserId);

        builder.HasIndex(application => application.SubmissionReference)
            .IsUnique()
            .HasFilter("[SubmissionReference] IS NOT NULL");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(application => application.CustomerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<LoanProduct>()
            .WithMany()
            .HasForeignKey(application => application.LoanProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(application => application.FinancialProfile)
            .WithOne()
            .HasForeignKey<LoanApplicationFinancialProfile>(profile => profile.LoanApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(application => application.ApplicantSnapshot)
            .WithOne()
            .HasForeignKey<ApplicantSnapshot>(snapshot => snapshot.LoanApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(application => application.StatusHistory)
            .WithOne()
            .HasForeignKey(history => history.LoanApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
