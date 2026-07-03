using codeHappy.Data.Enums;
using codeHappy.Data.Models;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Dtos.Snippet;

public record SnippetResponse
(
    string? Title,
    string? Description,
    SnippetVisibility Visibility,
    string? Topics,
    List<BlocksResponse> Blocks,
    Space? Space,
    Group? Groups
);