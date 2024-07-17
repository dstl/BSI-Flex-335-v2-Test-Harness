// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class PredictionLocationValidator : AbstractValidator<DetectionReport.Types.PredictedLocation>
    {
        public PredictionLocationValidator() 
        {
            RuleFor(x => new { x.Location, x.RangeBearing })
                .Must(x => x.Location != null || x.RangeBearing != null)
                .WithMessage("The PredictionLocation Location or RangeBearing is mandatory.");
            RuleFor(x => new { x.Location, x.RangeBearing })
                .Must(x => x.Location == null || x.RangeBearing == null)
                .WithMessage("The PredictionLocation must have either a Location or RangeBearing not both.");
            RuleFor(x => x.RangeBearing).SetValidator(new RangeBearingValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleFor(x => x.Location).SetValidator(new LocationValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
        }
    }
}
