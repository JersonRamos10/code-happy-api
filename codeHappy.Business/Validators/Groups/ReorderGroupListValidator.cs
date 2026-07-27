using FluentValidation;
using codeHappy.Business.Dtos.Groups;

namespace codeHappy.Business.Validators.Groups;

public class ReorderGroupListValidator : AbstractValidator<List<ReorderGroupRequest>>
{
    public ReorderGroupListValidator()
    {
        RuleFor(list => list)
            .NotEmpty()
            .WithMessage("Reorder list must contain at least one group");

        RuleFor(list => list)
            .Must(list => list.Select(r => r.Id).Distinct().Count() == list.Count)
            .WithMessage("Reorder list contains duplicate group Ids")
            .When(list => list is not null && list.Count > 0);

        RuleForEach(list => list)
            .SetValidator(new ReorderGroupsRequestValidator());
    }
}
