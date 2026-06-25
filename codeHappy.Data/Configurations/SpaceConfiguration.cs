using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace codeHappy.Data.Configurations;

public class SpaceConfiguration : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired().HasMaxLength(100);

        builder.Property(s => s.Icon)
            .HasMaxLength(50);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("now()");


        //Indices
        //para obtener todos los spaces del usuario
        builder.HasIndex(s => s.OwnerId);

        builder.HasIndex(s => new { s.OwnerId, s.LastAccessedAt });

        //Relacionchip 1:N with Groups

        builder.HasMany(s => s.Groups)
            .WithOne(g => g.Space)
            .HasForeignKey(sg => sg.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);

        //Relacionchip 1:N with Snippets
        builder.HasMany(s => s.Snippets)
           .WithOne(sn => sn.Space)
           .HasForeignKey(sn => sn.SpaceId)
           .OnDelete(DeleteBehavior.SetNull);

    }
}