using System.Runtime.Intrinsics.X86;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace codeHappy.Data.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.DisplayName)
           .IsRequired()
           .HasMaxLength(50);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(P => P.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("now()");

        //Index
        builder.HasIndex(p => p.UserName).IsUnique();
        builder.HasIndex(p => p.Email).IsUnique();

        //Relacionchip 1:N with Snippets
        builder.HasMany(p => p.Snippets)
            .WithOne(s => s.Profile)
            .HasForeignKey(ps => ps.OwnerId);

        //Relacionchip 1:N with Spaces
        builder.HasMany(p => p.Spaces)
            .WithOne(s => s.Profile)
            .HasForeignKey(s => s.OwnerId);

        //Relacionchip 1:N with Comments

        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Profile)
            .HasForeignKey(pc => pc.OwnerId);

        //Relacionchip 1:N with Shares

        builder.HasMany(p => p.Shares)
            .WithOne(s => s.Profile)
            .HasForeignKey(ps => ps.SharedBy);
    }

}