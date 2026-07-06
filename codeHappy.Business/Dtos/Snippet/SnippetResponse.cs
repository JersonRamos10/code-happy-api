using codeHappy.Data.Enums;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Dtos.Snippet;

public record SnippetResponse(
    Guid Id,
    string Title,
    string? Description,
    SnippetVisibility Visibility,
    List<string>? Topics,
    List<BlocksResponse> Blocks,
    Guid? SpaceId,
    Guid? GroupId
);