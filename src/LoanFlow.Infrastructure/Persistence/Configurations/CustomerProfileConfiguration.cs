using LoanFlow.Domain.Entities;
using LoanFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.UserId)
            .HasMaxLength(450);

        builder.Property(profile => profile.FullName)
            .HasMaxLength(CustomerProfile.FullNameMaxLength);

        builder.Property(profile => profile.DateOfBirth);

        builder.Property(profile => profile.NationalIdNumber)
            .HasMaxLength(CustomerProfile.NationalIdMaxLength);

        builder.Property(profile => profile.MobileNumber)
            .HasMaxLength(CustomerProfile.MobileNumberLength);

        builder.Property(profile => profile.CurrentAddress)
            .HasMaxLength(CustomerProfile.AddressMaxLength);

        builder.Property(profile => profile.PermanentAddress)
            .HasMaxLength(CustomerProfile.AddressMaxLength);

        builder.Property(profile => profile.RowVersion)
            .IsRowVersion();

        builder.HasIndex(profile => profile.UserId)
            .IsUnique();

        builder.HasOne<ApplicationUser>()
            .WithOne(user => user.CustomerProfile)
            .HasForeignKey<CustomerProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
