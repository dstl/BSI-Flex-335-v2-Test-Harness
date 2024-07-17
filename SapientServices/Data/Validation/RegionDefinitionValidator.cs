// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class RegionDefinitionValidator : AbstractValidator<Registration.Types.RegionDefinition>
    { 
        public RegionDefinitionValidator()
        {
            RuleFor(x => x.RegionType).NotNull().NotEmpty().WithMessage("The RegionDefinition {PropertyName} is mandatory.");
            RuleForEach(x => x.RegionType).NotNull().NotEmpty().SetValidator(new RegionTypeValidator()).WithMessage("The RegionDefinition {PropertyName} is invalid.");
            RuleFor(x => x.SettleTime).SetValidator(new DurationValidator()).WithMessage("The RegionDefinition {PropertyName} is invalid.");
            RuleFor(x => x.RegionArea).NotNull().NotEmpty().WithMessage("The RegionDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.RegionArea).NotNull().SetValidator(new LocationTypeValidator(false)).WithMessage("The RegionDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.ClassFilterDefinition).SetValidator(new ClassFilterDefinitionValidator()).WithMessage("The RegionDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.BehaviourFilterDefinition).SetValidator(new BehaviourFilterDefinitionValidator()).WithMessage("The RegionDefinition {PropertyName} is invalid.");
        }
    }
}