using codeHappy.Data.Enums;

namespace codeHappy.Business.Dtos.Blocks;

public record BlocksResponse(
    Guid Id,
    string? Title,
    string Content,
    string? Lenguaje,
    BlockType Type,
    DateTime CreatedAt,
    DateTime UpdateAt,
    int Position
);