namespace codeHappy.Business.Dtos.Groups;

public record GroupResponse(
    Guid Id,
    Guid SpaceId,
    string Name,
    int Position,
    DateTime CreatedAt
);
