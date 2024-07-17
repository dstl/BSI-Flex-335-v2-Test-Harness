// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class RegistrationDetectionReportValidator : AbstractValidator<Registration.Types.DetectionReport>
    {
        public RegistrationDetectionReportValidator()
        {
            RuleFor(x => x.Category).NotEmpty().NotNull().IsInEnum().WithMessage("The Location {PropertyName} is mandatory.");
            RuleFor(x => x.Category).NotEqual(Registration.Types.DetectionReportCategory.Unspecified).WithMessage("The Location {PropertyName} is mandatory.");
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The Location {PropertyName} is mandatory.");
            RuleFor(x => x.Units).NotEmpty().NotNull().WithMessage("The Location {PropertyName} is mandatory."); 
            RuleFor(x => x.HasCategory).Equal(true).WithMessage("The Location Category is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The Location Type is mandatory.");
            RuleFor(x => x.HasUnits).Equal(true).WithMessage("The Location Units is mandatory.");
        }
    }
}