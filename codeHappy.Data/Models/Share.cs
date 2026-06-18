using codeHappy.Data.Enums;
namespace codeHappy.Data.Models;

public class Share
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public SharePermission Permision { get; set; }

    public string SharedWith { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Relacionchips

    //N:1 with snippets

    public Guid SnippetId { get; set; }
    public Snippet Snippet { get; set; } = null!;


    public Guid SharedBy { get; set; }
    public Profile profile { get; set; } = null!;
}