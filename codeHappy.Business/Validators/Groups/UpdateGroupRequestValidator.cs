using codeHappy.Business.Dtos.Groups;
using FluentValidation;
namespace codeHappy.Business.Validators.Groups;

public class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters");
    }
}