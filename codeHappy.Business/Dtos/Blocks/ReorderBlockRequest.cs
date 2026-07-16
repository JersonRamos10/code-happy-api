namespace codeHappy.Business.Dtos.Blocks;

public record ReorderBlockRequest(
    Guid Id,
    int Position
);