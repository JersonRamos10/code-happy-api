using codeHappy.Business.Dtos.Profile;
using FluentValidation;

namespace codeHappy.Business.Validators.Profile;

public class SyncProfileRequestValidator : AbstractValidator<SyncProfileRequest>
{
    public  SyncProfileRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required")
            .MaximumLength(50);
        
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("DisplayName is required")
            .MaximumLength(50);
    }    
}