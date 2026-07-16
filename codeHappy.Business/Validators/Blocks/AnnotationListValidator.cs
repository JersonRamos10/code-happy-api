using FluentValidation;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Validators.Blocks;

public class AnnotationListValidator : AbstractValidator<List<CreateAnnotationRequest>>
{
    public AnnotationListValidator()
    {
        RuleForEach(list => list)
            .SetValidator(new CreateAnnotationRequestValidator());
    }
}
