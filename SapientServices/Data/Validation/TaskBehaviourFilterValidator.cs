// Crown-owned copyright, 2021-2024
using FluentValidation;
using static Sapient.Data.Task.Types;

namespace SapientServices.Data.Validation
{
    public class TaskBehaviourFilterValidator : AbstractValidator<BehaviourFilter>
    {
        public TaskBehaviourFilterValidator()
        {
            RuleFor(x => x.Parameter).NotEmpty().NotNull().WithMessage("The Task BehaviourFilter {PropertyName} is mandatory.");
            RuleFor(x => x.Parameter).SetValidator(new TaskParameterValidator()).WithMessage("The Task BehaviourFilter {PropertyName} is invalid.");
        }
    }
}
