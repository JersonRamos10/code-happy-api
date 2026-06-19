namespace codeHappy.Data.Models;

public class Comment
{
    public Guid Id { get; set; }

    public required string Text { get; set; }

    public required string OwnerName { get; set; }

    public string? OwnerAvatar { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    //N:1 with snippet
    public required Guid SnippetId { get; set; }
    public Snippet Snippet { get; set; } = null!;

    //N:1 with profile
    public required Guid OwnerId { get; set; }
    public Profile Profile { get; set; } = null!;
}