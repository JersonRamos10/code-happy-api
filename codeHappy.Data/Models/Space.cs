

namespace codeHappy.Data.Models;

public class Space
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public string? Icon { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdateAt { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    //Relacionchips

    //N:1 with profiles
    public Guid OwnerId { get; set; }

    public Profile Profile { get; set; } = null!;

    //1:N with groups

    public ICollection<Group> Groups { get; set; } = [];

    //1:N with snippets
    public ICollection<Snippet> Snippets { get; set; } = [];
}