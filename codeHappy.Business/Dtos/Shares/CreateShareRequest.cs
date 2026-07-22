namespace codeHappy.Business.Dtos.Shares;

public record CreateShareRequest
(
    Guid SnippetId,
    DateTime? ExpiresAt
);