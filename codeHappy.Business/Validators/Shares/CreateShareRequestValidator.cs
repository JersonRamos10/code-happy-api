using System.Data;
using codeHappy.Business.Dtos.Shares;
using FluentValidation;
namespace codeHappy.Business.Validators.Shares;

public class CreateShareRequestValidator : AbstractValidator<CreateShareRequest>
{
    public CreateShareRequestValidator()
    {
        RuleFor(s => s.SnippetId)
            .NotEmpty()
            .WithMessage("SnippetId is required.");

        RuleFor(s => s.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("ExpiresAt must be a future date.");
    }
}