namespace codeHappy.Business.Dtos.Groups;

public record ReorderGroupRequest(
    Guid Id,
    int Position
);
