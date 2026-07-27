using FluentValidation;
using codeHappy.Business.Dtos.Groups;

namespace codeHappy.Business.Validators.Groups;

public class ReorderGroupsRequestValidator : AbstractValidator<ReorderGroupRequest>
{
    public ReorderGroupsRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(r => r.Position)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Position must be greater or equal 0");
    }
}
