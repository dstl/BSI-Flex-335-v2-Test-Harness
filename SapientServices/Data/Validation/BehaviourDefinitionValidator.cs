// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class BehaviourDefinitionValidator : AbstractValidator<Registration.Types.BehaviourDefinition>
    {
        public BehaviourDefinitionValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The BehaviourDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The BehaviourDefinition Type is mandatory.");
        }
    }
}