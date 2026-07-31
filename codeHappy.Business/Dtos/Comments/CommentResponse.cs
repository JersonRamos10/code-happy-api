using codeHappy.Business.Dtos.Profile;

namespace codeHappy.Business.Dtos.Comments;

public record CommentResponse(
    Guid Id,
    Guid SnippetId,
    string Text,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid OwnerId,
    UserProfileResponse Profile
);