using codeHappy.Business.Dtos.Snippet;
using codeHappy.Business.Validators.Blocks;
using FluentValidation;

namespace codeHappy.Business.Validators.Snippets;

public class UpdateSnippetRequestValidator : AbstractValidator<UpdateSnippetRequest>
{
    public UpdateSnippetRequestValidator()
    {
        RuleFor(s => s.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(50)
                .WithMessage("Title cannot exceed 50 characters");

        RuleFor(s => s.Description)
                .MaximumLength(4000)
                .WithMessage("Description cannot exceed 4000 characters");

        RuleFor(s => s.Topics)
                .Must(t => t is null || t.Count <= 20)
                .WithMessage("Topics cannot exceed 20 items");

        RuleForEach(s => s.Topics)
                .NotEmpty()
                .WithMessage("Topic cannot be empty")
                .MaximumLength(30)
                .WithMessage("Topic cannot exceed 30 characters");

        RuleFor(s => s.Blocks)
                .NotEmpty()
                .WithMessage("Snippet must have at least one block");

        RuleForEach(s => s.Blocks)
                .SetValidator(new CreateBlockRequestValidator());
    }
}
