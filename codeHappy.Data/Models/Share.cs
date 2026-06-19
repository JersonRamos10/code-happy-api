using codeHappy.Data.Enums;
namespace codeHappy.Data.Models;

public class Share
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public SharePermission Permission { get; set; }

    public string SharedWith { get; set; } = string.Empty;

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    //N:1 with snippet
    public Guid SnippetId { get; set; }
    public Snippet Snippet { get; set; } = null!;

    //N:1 with profile
    public Guid SharedBy { get; set; }
    public Profile Profile { get; set; } = null!;
}