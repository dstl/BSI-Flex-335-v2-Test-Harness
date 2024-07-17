// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class DetectionDefinitionValidator : AbstractValidator<Registration.Types.DetectionDefinition>
    {
        public DetectionDefinitionValidator()
        {
            RuleFor(x => x.LocationType).NotNull().WithMessage("The DetectionDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.LocationType).SetValidator(new LocationTypeValidator(false)).WithMessage("The DetectionDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.DetectionPerformance).SetValidator(new PerformanceValueValidator()).WithMessage("The DetectionDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.DetectionReport).SetValidator(new RegistrationDetectionReportValidator()).WithMessage("The DetectionDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.DetectionClassDefinition).SetValidator(new DetectionClassDefinitionValidator()).WithMessage("The DetectionDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.BehaviourDefinition).SetValidator(new BehaviourDefinitionValidator()).WithMessage("The DetectionDefinition {PropertyName} is invalid.");
            RuleFor(x => x.VelocityType).SetValidator(new VelocityTypeValidator()).WithMessage("The DetectionDefinition {PropertyName} is invalid.");
            RuleFor(x => x.GeometricError).SetValidator(new GeometricErrorValidator()).WithMessage("The DetectionDefinition {PropertyName} is invalid.");
        }
    }
}