using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

public class ApplicantSnapshotConfiguration : IEntityTypeConfiguration<ApplicantSnapshot>
{
    public void Configure(EntityTypeBuilder<ApplicantSnapshot> builder)
    {
        builder.HasKey(snapshot => snapshot.Id);

        builder.Property(snapshot => snapshot.FullName)
            .HasMaxLength(CustomerProfile.FullNameMaxLength);

        builder.Property(snapshot => snapshot.NationalIdNumber)
            .HasMaxLength(CustomerProfile.NationalIdMaxLength);

        builder.Property(snapshot => snapshot.MobileNumber)
            .HasMaxLength(CustomerProfile.MobileNumberLength);

        builder.Property(snapshot => snapshot.CurrentAddress)
            .HasMaxLength(CustomerProfile.AddressMaxLength);

        builder.Property(snapshot => snapshot.PermanentAddress)
            .HasMaxLength(CustomerProfile.AddressMaxLength);

        builder.HasIndex(snapshot => snapshot.LoanApplicationId)
            .IsUnique();
    }
}
