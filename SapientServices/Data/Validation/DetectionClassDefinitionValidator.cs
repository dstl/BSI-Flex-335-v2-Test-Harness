// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class DetectionClassDefinitionValidator : AbstractValidator<Registration.Types.DetectionClassDefinition>
    {
        public DetectionClassDefinitionValidator()
        {
            RuleFor(x => x.ConfidenceDefinition).IsInEnum().WithMessage("The DetectionClass {PropertyName} is invalid.");
            RuleForEach(x => x.ClassPerformance).SetValidator(new PerformanceValueValidator()).WithMessage("The DetectionClass {PropertyName} is invalid.");
            RuleForEach(x => x.ClassDefinition).SetValidator(new ClassDefinitionValidator()).WithMessage("The DetectionClass {PropertyName} is invalid.");
            RuleForEach(x => x.TaxonomyDockDefinition).SetValidator(new TaxonomyDockDefinitionValidator()).WithMessage("The DetectionClass {PropertyName} is invalid.");
        }
    }
}