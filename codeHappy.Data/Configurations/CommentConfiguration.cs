using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codeHappy.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Text)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.OwnerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.OwnerAvatar)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("now()");


        //index
        builder.HasIndex(c => c.SnippetId);
        builder.HasIndex(c => new { c.SnippetId, c.CreatedAt });
    }
}
