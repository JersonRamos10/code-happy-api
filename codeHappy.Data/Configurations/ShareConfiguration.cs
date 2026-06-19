using codeHappy.Data.Enums;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codeHappy.Data.Configurations;

public class ShareConfiguration : IEntityTypeConfiguration<Share>
{
    public void Configure(EntityTypeBuilder<Share> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Permission)
            .HasConversion<string>()
            .HasDefaultValue(SharePermission.Viewer);

        builder.Property(s => s.SharedWith)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.ExpiresAt)
            .IsRequired(false);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("now()");

        builder.HasIndex(s => s.SnippetId);
        builder.HasIndex(s => s.SharedBy);
        builder.HasIndex(s => new { s.SnippetId, s.SharedWith });
    }
}
