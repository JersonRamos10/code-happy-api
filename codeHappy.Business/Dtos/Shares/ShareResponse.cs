namespace codeHappy.Business.Dtos.Shares;

public record ShareResponse(
    Guid Id,
    Guid SnippetId,
    DateTime? ExpiresAt,
    DateTime CreatedAt
    
);