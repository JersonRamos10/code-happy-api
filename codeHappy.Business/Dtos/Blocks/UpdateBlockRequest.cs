using codeHappy.Data.Models;
namespace CodeHappy.Business.Dtos.Blocks;

public record UpdateBlockRequest (
    string? Title,
    string? Content,
    string? Language,
    List<CodeAnnotation>? Annotations,
    int? Width,
    int? Height,
    string? Alt,
    string? BucketPath
);