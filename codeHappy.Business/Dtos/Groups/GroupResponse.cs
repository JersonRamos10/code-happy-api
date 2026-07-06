namespace codeHappy.Business.Dtos.Groups;

public record GroupResponse(
    Guid Id,
    string Name,
    int Position,
    DateTime CreatedAt
);