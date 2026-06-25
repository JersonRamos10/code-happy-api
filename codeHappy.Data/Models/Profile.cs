using System.ComponentModel.DataAnnotations;

namespace codeHappy.Data.Models;

public class Profile
{
    //Properties
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string UserName { get; set; }

    public required string DisplayName { get; set; }

    public required string Email { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    //Relacionchips

    //1:N with spaces

    public ICollection<Space> Spaces { get; set; } = [];

    //1:N with snippets

    public ICollection<Snippet> Snippets { get; set; } = [];


    //1:N with comments

    public ICollection<Comment> Comments { get; set; } = [];

    //1:N with shares

    public ICollection<Share> Shares { get; set; } = [];

}