using codeHappy.Data.Enums;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Dtos.Snippet;

public record SnippetResponse(
    Guid Id,
    string Title,
    string? Description,
    SnippetVisibility Visibility,
    bool IsFavorite,
    int ViewCount,
    int CopyCount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<string>? Topics,
    List<BlocksResponse> Blocks,
    Guid? SpaceId,
    Guid? GroupId
);