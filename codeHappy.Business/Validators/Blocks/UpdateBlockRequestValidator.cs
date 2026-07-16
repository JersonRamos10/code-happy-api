using codeHappy.Business.Dtos.Blocks;
using FluentValidation;
namespace codeHappy.Business.Validators.Blocks;

public class UpdateBlockRequestValidator : AbstractValidator<UpdateBlockRequest>
{
    public UpdateBlockRequestValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty()
            .When(b  => b.Title != null)
            .WithMessage("Title not empty");
        
        RuleFor(b => b.Content)
            .NotEmpty()
            .WithMessage("Content not empty")
            .When(b => b.Content != null);

        RuleFor(b => b.Language).NotEmpty()
            .WithMessage("Language not empty")
            .When(b => b.Language != null);
    }
}