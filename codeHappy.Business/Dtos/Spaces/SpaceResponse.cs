namespace codeHappy.Business.Dtos.Spaces;

public record SpaceResponse(
    Guid Id,
    string Name,
    string? Icon,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? LastAccessedAt

);