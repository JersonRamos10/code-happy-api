using codeHappy.Business.Dtos.Spaces;
using FluentValidation;

namespace codeHappy.Business.Validators.Spaces;

public class UpdateSpaceRequestValidator : AbstractValidator<UpdateSpaceRequest>
{
    public UpdateSpaceRequestValidator()
    {
        RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name spaces is Required")
                .MaximumLength(50).WithMessage("Name space i'snt more 50 characters");

        RuleFor(x => x.Icon)
                .MaximumLength(30)
                .When(x => x.Icon != null);
    }
}