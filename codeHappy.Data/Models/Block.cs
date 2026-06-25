using codeHappy.Data.Enums;

namespace codeHappy.Data.Models;

public class Block
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public BlockType Type { get; set; }

    public required string Content { get; set; }

    public string? Language { get; set; }

    public string? Title { get; set; }

    public int Position { get; set; }

    public List<CodeAnnotation> Annotations { get; set; } = [];

    public ImageMetadata? ImageMetadata { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    //relacionchips

    //N:1 with snippets

    public required Guid SnippetId { get; set; }

    public Snippet Snippet { get; set; } = null!;


}