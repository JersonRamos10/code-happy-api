using codeHappy.Data.Enums;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Dtos.Snippet;

public record CreateSnippetRequest(
    string Title,
    string? Description,
    SnippetVisibility Visibility,
    List<CreateBlockRequest> Blocks,
    Guid? SpaceId,
    Guid? GroupId,
    List<string>? Topics
);