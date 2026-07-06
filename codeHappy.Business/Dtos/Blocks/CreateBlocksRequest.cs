using codeHappy.Data.Enums;

namespace codeHappy.Business.Dtos.Blocks;

public record CreateBlocksRequest(
    string? Title,
    string Content,
    BlockType Type,
    string? Language
);