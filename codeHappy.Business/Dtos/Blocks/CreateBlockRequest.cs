using codeHappy.Data.Enums;


namespace codeHappy.Business.Dtos.Blocks;

public record CreateBlockRequest(
    string? Title,
    string Content,
    BlockType Type,
    int Position,
    string? Language,
    List<CreateAnnotationRequest>? Annotations
);