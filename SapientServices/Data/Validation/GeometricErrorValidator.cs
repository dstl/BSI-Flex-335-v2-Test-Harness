// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class GeometricErrorValidator : AbstractValidator<Registration.Types.GeometricError>
    {
        public GeometricErrorValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The Location {PropertyName} is mandatory.");
            RuleFor(x => x.Units).NotEmpty().NotNull().WithMessage("The Location {PropertyName} is mandatory.");
            RuleFor(x => x.VariationType).NotEmpty().NotNull().WithMessage("The Location {PropertyName} is mandatory."); 
            RuleFor(x => x.HasType).Equal(true).WithMessage("The Location Type is mandatory.");
            RuleFor(x => x.HasUnits).Equal(true).WithMessage("The Location Units is mandatory.");
            RuleFor(x => x.HasVariationType).Equal(true).WithMessage("The Location Variation is mandatory.");
            RuleForEach(x => x.PerformanceValue).SetValidator(new PerformanceValueValidator()).WithMessage("The GeometricError {PropertyName} is invalid.");
        }
    }
}