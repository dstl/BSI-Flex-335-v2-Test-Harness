// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class TaskDefinitionValidator : AbstractValidator<Registration.Types.TaskDefinition>
    {
        public TaskDefinitionValidator()
        {
            RuleFor(x => x.ConcurrentTasks).NotNull().WithMessage("The TaskDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasConcurrentTasks).Equal(true).WithMessage("The TaskDefinition ConcurrentTasks is mandatory.");
            RuleFor(x => x.RegionDefinition).NotNull().NotEmpty().SetValidator(new RegionDefinitionValidator()).WithMessage("The TaskDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.Command).SetValidator(new CommandValidator()).WithMessage("The TaskDefinition {PropertyName} is invalid.");
        }
    }
}