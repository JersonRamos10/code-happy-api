using System.Net.Mime;
using codeHappy.Data.Enums;

namespace codeHappy.Business.Dtos.Blocks;

public record BlocksRequest(
    string? Title,
    string Content,
    BlockType Type
);