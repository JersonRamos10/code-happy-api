using FluentValidation;
using codeHappy.Business.Dtos.Blocks;

namespace codeHappy.Business.Validators.Blocks;

public class ReorderBlockListValidator : AbstractValidator<List<ReorderBlockRequest>>
{
    public ReorderBlockListValidator()
    {
        RuleFor(list => list)
            .NotEmpty()
            .WithMessage("Reorder list must contain at least one block");

        RuleFor(list => list)
            .Must(list => list.Select(r => r.Id).Distinct().Count() == list.Count)
            .WithMessage("Reorder list contains duplicate block Ids")
            .When(list => list is not null && list.Count > 0);

        RuleForEach(list => list)
            .SetValidator(new ReorderBlocksRequestValidator());
    }
}
