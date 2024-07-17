// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class AlertValidator : AbstractValidator<Alert>
    {
        public AlertValidator()
        {
            RuleFor(x => x.AlertId).NotEmpty().IsValidUlid().WithMessage("The Alert {PropertyName} is mandatory.");
            RuleFor(x => x.Location)
                .NotNull()
                .When(x => x.LocationOneofCase == Alert.LocationOneofOneofCase.Location)
                .WithMessage("The Alert {PropertyName} is invalid.");
            RuleFor(x => x.Location)
                .SetValidator(new LocationValidator())
                .When(x => x.LocationOneofCase == Alert.LocationOneofOneofCase.Location)
                .WithMessage("The Alert {PropertyName} is invalid.");
            RuleFor(x => x.RangeBearing)
                .NotNull()
                .When(x => x.LocationOneofCase == Alert.LocationOneofOneofCase.RangeBearing)
                .WithMessage("The Alert {PropertyName} is invalid.");
            RuleFor(x => x.RangeBearing)
                .SetValidator(new RangeBearingValidator())
                .When(x => x.LocationOneofCase == Alert.LocationOneofOneofCase.RangeBearing)
                .WithMessage("The Alert {PropertyName} is invalid.");
            RuleFor(x => x.Ranking).Must(CommonValidator.BeAValidScalar).WithMessage("The Alert {PropertyName} is invalid."); ;
            RuleFor(x => x.Confidence).Must(CommonValidator.BeAValidScalar).WithMessage("The Alert {PropertyName} is invalid."); ;
            RuleFor(x => x.RegionId).IsValidUlid().WithMessage("The Alert {PropertyName} is invalid."); ;
            RuleForEach(x => x.AssociatedFile).SetValidator(new AssociatedFileValidator()).WithMessage("The Alert {PropertyName} is invalid."); ;
            RuleForEach(x => x.AssociatedDetection).SetValidator(new AssociatedDetectionValidator()).WithMessage("The Alert {PropertyName} is invalid."); ;
        }
    }
}
