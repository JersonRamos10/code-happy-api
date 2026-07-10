using codeHappy.Data.Enums;
using codeHappy.Data.Models;


namespace codeHappy.Business.Dtos.Blocks;

public record CreateBlockRequest(
    string? Title,
    string Content,
    BlockType Type,
    string? Language,
    List<CodeAnnotation>? Annotations,
    int? Width,
    int? Height,
    string? Alt,
    string? BucketPath
);