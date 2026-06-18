using codeHappy.Data.Enums;

namespace codeHappy.Data.Models;

public class Snippet
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required Visibility Visibility { get; set; }

    public bool IsFavorite { get; set; }

    public int ViewCount { get; set; }

    public int CopyCount { get; set; }

    public IList<string> Topics { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdateAt { get; set; }


    //Relacionchips

    //N:1 with profile

    public Guid OwnerId { get; set; }

    public Profile Profile { get; set; } = null!;

    //N:1 with Space

    public Guid? SpaceId { get; set; }

    public Space? Space { get; set; }

    //N:1 with group

    public Guid? GroupId { get; set; }

    public Group? Group { get; set; }


    //1:N with blocks

    public ICollection<Block> Blocks { get; set; } = [];

    //1:N with comments

    public ICollection<Comment> Comments { get; set; } = [];

    //1:N with shares

    public ICollection<Share> Shares { get; set; } = [];
}