using codeHappy.Data.Enums;

namespace codeHappy.Business.Dtos.Snippet;

public record SnippetSummaryResponse(
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
    Guid? SpaceId,
    Guid? GroupId
);
