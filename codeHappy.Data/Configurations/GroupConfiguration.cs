using System.Net;
using codeHappy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql;

namespace codeHappy.Data.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{

    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.CreatedAt)
          .HasDefaultValueSql("now()");

        builder.Property(g => g.UpdatedAt)
            .HasDefaultValueSql("now()");

        //Index
        builder.HasIndex(g => new { g.SpaceId, g.Position });

        //Constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Group_Position",
                "\"Position\" >= 0");
        });

        //Relacionchip 1:N with Snippets

        builder.HasMany(s => s.Snippets)
               .WithOne(g => g.Group)
               .HasForeignKey(gs => gs.GroupId)
               .OnDelete(DeleteBehavior.SetNull);
        ;

    }

}