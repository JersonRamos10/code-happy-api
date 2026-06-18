namespace codeHappy.Data.Models;

public class Annotation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int LineNumber { get; set; }

    public required string Text { get; set; }
}