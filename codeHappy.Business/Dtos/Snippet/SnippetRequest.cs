using codeHappy.Data.Models;
using codeHappy.Data.Enums;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Dtos.Snippet;

public record SnippetRequest(
    string Title,
    string? Description,
    SnippetVisibility Visibility,
    List<BlocksRequest> Blocks,
    Space? Space,
    Group? Group,
    string? Topics


);