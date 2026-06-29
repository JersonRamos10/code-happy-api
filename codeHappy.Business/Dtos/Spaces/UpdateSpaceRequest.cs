namespace codeHappy.Business.Dtos.Spaces;

public record UpdateSpaceRequest(
    string Name,
    string? Icon
);