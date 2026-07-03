using codeHappy.Business.Dtos.Pagined;

namespace codeHappy.Business.Dtos.Snippet;

public record SnippetParamsRequest(
    Guid? SpaceId,
    Guid? GroupId,
    bool? IsFavorite,
    int PageNumber = 1,
    int PageSize = 20
) : PagedRequest(PageNumber, PageSize);