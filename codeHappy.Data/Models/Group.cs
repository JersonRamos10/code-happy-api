using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace codeHappy.Data.Models;

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public int Position { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdateAt { get; set; }


    //Relacionchips

    //N:1 with profile

    public Guid OwnerId { get; set; }

    public Profile Profile { get; set; } = null!;

    //N:1 with spaces

    public Guid SpaceId { get; set; }

    public Space Space { get; set; } = null!;


    //1:N with snippets

    public ICollection<Snippet> Snippets { get; set; } = [];

}