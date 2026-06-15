using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

public class LoanApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<LoanApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<LoanApplicationStatusHistory> builder)
    {
        builder.HasKey(history => history.Id);

        builder.Property(history => history.Note)
            .HasMaxLength(250);
    }
}
