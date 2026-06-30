namespace codeHappy.Business.Dtos.Group;

public record GroupResponse(
    Guid Id,
    string Name,
    int Position,
    DateTime CreatedAt
);