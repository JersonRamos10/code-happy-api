using codeHappy.Data.Models;
using codeHappy.Data.Configurations;

using Microsoft.EntityFrameworkCore;

namespace codeHappy.Data.Context;

public class CodeHappyContext : DbContext
{

    public DbSet<Profile> Profiles { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<Space> Spaces { get; set; }

    public DbSet<Snippet> Snippets { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Share> Shares { get; set; }

    public CodeHappyContext(DbContextOptions<CodeHappyContext> options) : base(options)
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfileConfiguration).Assembly);

    }


}