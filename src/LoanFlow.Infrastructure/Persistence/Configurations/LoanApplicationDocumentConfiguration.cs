using LoanFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

public class LoanApplicationDocumentConfiguration : IEntityTypeConfiguration<LoanApplicationDocument>
{
    public void Configure(EntityTypeBuilder<LoanApplicationDocument> builder)
    {
        builder.HasKey(document => document.Id);

        builder.Property(document => document.Note)
            .HasMaxLength(250);

        builder.HasIndex(document => new { document.LoanApplicationId, document.DocumentType })
            .IsUnique();
    }
}
