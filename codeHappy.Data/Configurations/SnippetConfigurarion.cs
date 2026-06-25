using codeHappy.Data.Enums;
using codeHappy.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codeHappy.Data.Configurations;

public class SnippetConfiguration : IEntityTypeConfiguration<Snippet>
{
    public void Configure(EntityTypeBuilder<Snippet> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .IsRequired(false)
            .HasMaxLength(4000);

        builder.Property(s => s.Visibility)
            .HasConversion<string>()
            .HasDefaultValue(SnippetVisibility.Private);

        builder.Property(s => s.Topics)
            .HasColumnType("jsonb");

        builder.Property(s => s.IsFavorite)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.CopyCount)
           .IsRequired()
           .HasDefaultValue(0);

        builder.Property(P => P.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("now()");


        //Relacionchip 1:N with Blocks
        builder.HasMany(s => s.Blocks)
               .WithOne(b => b.Snippet)
               .HasForeignKey(sb => sb.SnippetId)
               .OnDelete(DeleteBehavior.Cascade);

        //Relacionchip 1:N with Snippets
        builder.HasMany(s => s.Comments)
               .WithOne(c => c.Snippet)
               .HasForeignKey(sc => sc.SnippetId)
               .OnDelete(DeleteBehavior.Cascade);

        //Relacionchip 1:N with Shares
        builder.HasMany(s => s.Shares)
            .WithOne(sh => sh.Snippet)
            .HasForeignKey(sh => sh.SnippetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}