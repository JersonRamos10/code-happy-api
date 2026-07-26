namespace codeHappy.Data.Models;

public class ImageMetadata

{
    public required string PublicId { get; set; }

    public required string SecureUrl { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public string? Format { get; set; }

    public long? Bytes { get; set; }

    public string? Alt { get; set; }

    public string? BucketPath { get; set; }
}



