using codeHappy.Business.Dtos.Blocks;
using codeHappy.Data.Enums;
using FluentValidation;

namespace codeHappy.Business.Validators.Blocks;

public class CreateBlockRequestValidator : AbstractValidator<CreateBlockRequest>
{
    public CreateBlockRequestValidator()
    {
        RuleFor(b => b.Content)
                .NotEmpty()
                .WithMessage("Block content is required");

        RuleFor(b => b.Language)
            .NotEmpty()
            .WithMessage("Language is required for code blocks")
            .When(b => b.Type == BlockType.Code);

        RuleFor(b => b.PublicId)
            .NotEmpty()
            .WithMessage("PublicId is required")
            .When(b => b.Type == BlockType.Image);

        RuleFor(b => b.Position)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Position must be zero or greater");

        RuleForEach(b => b.Annotations)
            .SetValidator(new CreateAnnotationRequestValidator());
    }
}