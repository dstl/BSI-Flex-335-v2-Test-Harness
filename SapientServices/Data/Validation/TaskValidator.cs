// Crown-owned copyright, 2021-2024
using FluentValidation;
using static Sapient.Data.Task.Types;

namespace SapientServices.Data.Validation
{
    public class TaskValidator : AbstractValidator<Sapient.Data.Task>
    {
        public TaskValidator()
        {
            RuleFor(x => x.TaskId).NotEmpty().NotNull().IsValidUlid().WithMessage("The Task {PropertyName} is mandatory.");
            RuleFor(x => x.Control).NotEmpty().NotNull().IsInEnum().NotEqual(Control.Unspecified).WithMessage("The TaskValidator {PropertyName} is mandatory.");
            RuleForEach(x => x.Region).SetValidator(new TaskRegionValidator()).WithMessage("The Task {PropertyName} is invalid.");
            RuleFor(x => x.Command).SetValidator(new TaskCommandValidator()).WithMessage("The Task {PropertyName} is invalid.");
        }
    }
}
