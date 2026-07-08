using codeHappy.Business.Dtos.Blocks;
using codeHappy.Data.Enums;
using FluentValidation;

namespace codeHappy.Business.Validators.Blocks;

public class CreateBlockRequestValidator : AbstractValidator<CreateBlocksRequest>
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
    }
}