using System.Data;
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
                .WithMessage("");

        RuleFor(b => b.Type)
            .NotEmpty()
            .When(b => b.Type == BlockType.Code);
    }
}