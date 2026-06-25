namespace codeHappy.Data.Models;

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public int Position { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    //Relacionchips

    //N:1 with spaces

    public Guid SpaceId { get; set; }

    public Space Space { get; set; } = null!;

    //1:N with snippets

    public ICollection<Snippet> Snippets { get; set; } = [];

}