using FluentValidation;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Validators.Blocks;

public class ReorderBlocksRequestValidator: AbstractValidator<ReorderBlockRequest>
{
    public ReorderBlocksRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(r => r.Position)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Position must be greater or equal 0");
        
    }
}