using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codeHappy.Data.Configurations;

public class ShareConfiguration : IEntityTypeConfiguration<Share>
{
    public void Configure(EntityTypeBuilder<Share> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.ExpiresAt)
            .IsRequired(false);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("now()");

        //index
        builder.HasIndex(s => s.SnippetId);
        builder.HasIndex(s => s.SharedBy);
    }
}
