using codeHappy.Business.Dtos.Snippet;
using FluentValidation;

namespace codeHappy.Business.Validators.Snippets;

public class MoveSnippetRequestValidator : AbstractValidator<MoveSnippetRequest>
{
    public MoveSnippetRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.HasLocationField)
            .WithMessage("At least one location field is required.");

        RuleFor(x => x.SpaceId)
            .NotEmpty()
            .When(x => x.GroupId.HasValue)
            .WithMessage("SpaceId is required when GroupId is provided.");
    }
}
