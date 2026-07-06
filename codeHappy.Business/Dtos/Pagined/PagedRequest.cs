namespace codeHappy.Business.Dtos.Pagined;

public record PagedRequest(
    int PageNumber = 1,
    int PageSize = 20
);
