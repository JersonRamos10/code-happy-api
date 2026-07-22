using codeHappy.Data.Enums;


namespace codeHappy.Business.Dtos.Blocks;

public record CreateBlockRequest(
    string? Title,
    string Content,
    BlockType Type,
    int Position,
    string? Language,
    string? PublicId,
    int? Width,
    int? Height,
    string? Format,
    long? Bytes,
    List<CreateAnnotationRequest>? Annotations
);