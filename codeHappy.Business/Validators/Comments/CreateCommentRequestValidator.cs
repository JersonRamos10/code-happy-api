using codeHappy.Business.Dtos.Comments;
using FluentValidation;

namespace codeHappy.Business.Validators.Comments;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(c => c.Text)
            .NotEmpty()
            .WithMessage("Text is required")
            .MaximumLength(10000)
            .WithMessage("Text cannot be longer than 10000");
    }

}