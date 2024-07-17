// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class BehaviourFilterDefinitionValidator : AbstractValidator<Registration.Types.BehaviourFilterDefinition>
    {
        public BehaviourFilterDefinitionValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The BehaviourFilterDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The BehaviourFilterDefinition Type is mandatory.");
            RuleForEach(x => x.FilterParameter).SetValidator(new FilterParameterValidator()).WithMessage("The BehaviourFilterDefinition {PropertyName} is invalid.");
        }
    }
}