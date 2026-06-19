using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codeHappy.Data.Configurations;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(b => b.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(b => b.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(b => b.Language)
            .HasMaxLength(50);

        builder.Property(b => b.Position)
            .IsRequired();

        builder.Property(b => b.Annotations)
            .HasColumnType("jsonb");

        builder.Property(g => g.CreatedAt)
         .HasDefaultValueSql("now()");

        builder.Property(b => b.UpdatedAt)
            .HasDefaultValueSql("now()");

        builder.OwnsOne(b => b.ImageMetadata, owned =>
        {
            owned.ToJson();
            owned.Property(im => im.Width);
            owned.Property(im => im.Height);
            owned.Property(im => im.Alt)
                .HasMaxLength(200);
            owned.Property(im => im.BucketPath)
                .HasMaxLength(500);
        });

        //Index
        builder.HasIndex(b => new { b.SnippetId, b.Position });
    }
}