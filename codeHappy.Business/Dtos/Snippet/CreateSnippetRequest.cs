using codeHappy.Data.Enums;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Dtos.Snippet;

public record CreateSnippetRequest(
    string Title,
    string? Description,
    SnippetVisibility Visibility,
    List<CreateBlocksRequest> Blocks,
    Guid? SpaceId,
    Guid? GroupId,
    string? Topics
);