// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class TaskParameterValidator : AbstractValidator<Task.Types.Parameter>
    { 
        public TaskParameterValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("The Task Parameter {PropertyName} is mandatory.");
            RuleFor(x => x.Operator).NotNull().NotEmpty().WithMessage("The Task Parameter {PropertyName} is mandatory.");
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage("The Task Parameter {PropertyName} is mandatory.");
            RuleFor(x => x.HasName).Equal(true).WithMessage("The Task Parameter Name is mandatory.");
            RuleFor(x => x.HasOperator).Equal(true).WithMessage("The Task Parameter Operator is mandatory.");
            RuleFor(x => x.HasValue).Equal(true).WithMessage("The Task Parameter Value is mandatory.");
        }
    }
}