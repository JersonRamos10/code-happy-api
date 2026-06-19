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

        builder.Property(g => g.Position)
            .IsRequired();

        builder.Property(g => g.CreatedAt)
          .HasDefaultValueSql("now()");

        builder.Property(g => g.UpdatedAt)
            .HasDefaultValueSql("now()");

        //Index
        builder.HasIndex(g => new { g.SpaceId, g.Position });


        //Relacionchip 1:N with Snippets

        builder.HasMany(s => s.Snippets)
               .WithOne(g => g.Group)
               .HasForeignKey(gs => gs.GroupId)
               .OnDelete(DeleteBehavior.Restrict);
        ;

        //Relacionchip N:1 with profile

        builder.HasOne(g => g.Profile)
                .WithMany()
                .HasForeignKey(gp => gp.OwnerId);


    }

}