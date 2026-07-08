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

        RuleFor(s => s.Blocks)
                .NotEmpty()
                .WithMessage("Snippet must have at least one block");

        RuleForEach(s => s.Blocks)
                .SetValidator(new CreateBlockRequestValidator());
    }
}
