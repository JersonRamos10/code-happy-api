using Npgsql.Internal;

namespace codeHappy.Data.Models;

public class Comment
{
    public Guid Id { get; set; }

    public required string Text { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    //Relacionchips

    //N:1 with snippet

    public required Guid SnippetsId { get; set; }

    public Snippet Snippet { get; set; } = null!;

    //N:1 with profile
    public required Guid OwnerId { get; set; }

    public Profile Profile { get; set; } = null!;
}