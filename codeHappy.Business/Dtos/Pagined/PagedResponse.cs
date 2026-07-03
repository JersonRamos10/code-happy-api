namespace codeHappy.Business.Dtos.Pagined;

public record PagedResponse<T>(
    List<T> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    int TotalPages
);
