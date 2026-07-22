namespace codeHappy.Data.Models;

public class Share
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    //N:1 with snippet
    public Guid SnippetId { get; set; }
    public Snippet Snippet { get; set; } = null!;

    //N:1 with profile
    public Guid SharedBy { get; set; }
    public Profile Profile { get; set; } = null!;
}