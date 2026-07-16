using codeHappy.Business.Dtos.Blocks;
using FluentValidation;

namespace codeHappy.Business.Validators.Blocks;

public class CreateAnnotationRequestValidator : AbstractValidator<CreateAnnotationRequest>
{
    public CreateAnnotationRequestValidator()
    {
        RuleFor(a => a.LineNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Line number must be zero or greater");
        
        RuleFor(a => a.Text)
            .NotEmpty()
            .WithMessage("Text must not be empty");
    }    
}