using codeHappy.Business.Dtos.Spaces;
using FluentValidation;

namespace codeHappy.Business.Validators.Spaces;

public class CreateSpaceRequestValidator : AbstractValidator<CreateSpaceRequest>
{
    public CreateSpaceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del space es obligatorio.")
            .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

        RuleFor(x => x.Icon)
            .MaximumLength(30).WithMessage("El icon no puede superar 30 caracteres.")
            .When(x => x.Icon != null);
    }
}