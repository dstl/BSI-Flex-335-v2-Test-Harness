// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class StatusDefinitionValidator : AbstractValidator<Registration.Types.StatusDefinition>
    {
        public StatusDefinitionValidator()
        {
            RuleFor(x => x.StatusInterval).NotNull().SetValidator(new DurationValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleFor(x => x.LocationDefinition).SetValidator(new LocationTypeValidator(true)).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleFor(x => x.CoverageDefinition).SetValidator(new LocationTypeValidator(false)).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleFor(x => x.ObscurationDefinition).SetValidator(new LocationTypeValidator(false)).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.StatusReport).SetValidator(new RegistrationStatusReportValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleFor(x => x.FieldOfViewDefinition).SetValidator(new LocationTypeValidator(false)).WithMessage("The StatusDefinition {PropertyName} is invalid.");
        }
    }
}