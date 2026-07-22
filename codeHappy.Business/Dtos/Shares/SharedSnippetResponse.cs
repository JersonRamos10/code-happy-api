using codeHappy.Business.Dtos.Blocks;
using codeHappy.Data.Enums;
namespace codeHappy.Business.Dtos.Shares;

public record SharedSnippetResponse(
    Guid Id,
    string Title,
    string? Description,
    SnippetVisibility Visibility,
    List<string>? Topics,
    List<BlocksResponse> Blocks
    );