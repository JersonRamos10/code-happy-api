using codeHappy.Business.Dtos.Snippet;
using FluentValidation;

namespace codeHappy.Business.Validators.Snippets;

public class CreateSnippetRequestValidator : AbstractValidator<CreateSnippetRequest>
{
    public CreateSnippetRequestValidator()
    {
        RuleFor(s => s.Title)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Title is Required");

        RuleFor(s => s.Blocks)
                .NotEmpty();
    }

}