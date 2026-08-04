using codeHappy.Data.Enums;
using codeHappy.Data.Models;

namespace codeHappy.Business.Dtos.Blocks;

public record BlocksResponse(
    Guid Id,
    string? Title,
    string Content,
    string? Language,
    BlockType Type,
    List<AnnotationResponse>? Annotations,
    int Position,
    ImageMetadataResponse? ImageMetadata,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
