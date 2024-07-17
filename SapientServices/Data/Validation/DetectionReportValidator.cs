// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class DetectionReportValidator : AbstractValidator<DetectionReport>
    {
        public DetectionReportValidator() 
        {
            RuleFor(x => x.ReportId).NotEmpty().NotNull().IsValidUlid().WithMessage("The DetectionReport {PropertyName} is mandatory.");
            RuleFor(x => x.ObjectId).NotEmpty().NotNull().IsValidUlid().WithMessage("The DetectionReport {PropertyName} is mandatory.");
            RuleFor(x => x.TaskId).IsValidUlid();
            RuleFor(x => new { x.Location, x.RangeBearing })
                .Must(x => x.Location != null || x.RangeBearing != null)
                .WithMessage("The DetectionReport Location or RangeBearing is mandatory.");
            RuleFor(x => new { x.Location, x.RangeBearing })
                .Must(x => x.Location == null || x.RangeBearing == null)
                .WithMessage("The DetectionReport must have either a Location or RangeBearing not both.");
            RuleFor(x => x.RangeBearing).SetValidator(new RangeBearingValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleFor(x => x.Location).SetValidator(new LocationValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleFor(x => x.DetectionConfidence).Must(CommonValidator.BeAValidScalar).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.TrackInfo).SetValidator(new TrackObjectInfoValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleFor(x => x.PredictionLocation).SetValidator(new PredictionLocationValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.ObjectInfo).SetValidator(new TrackObjectInfoValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.Classification).SetValidator(new DetectionReportClassificationValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.Behaviour).SetValidator(new BehaviourValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.AssociatedFile).SetValidator(new AssociatedFileValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.Signal).SetValidator(new SignalValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.AssociatedDetection).SetValidator(new AssociatedDetectionValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleForEach(x => x.DerivedDetection).SetValidator(new DerivedDetectionValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            RuleFor(x => x.EnuVelocity).SetValidator(new ENUVelocityValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
            //RuleFor(x => x.DetectionLocation).NotEmpty().NotNull().WithMessage("The DetectionReport {PropertyName} is mandatory.");
            //RuleFor(x => x.DetectionLocation).SetValidator(new LocationOrRangeBearingValidator()).WithMessage("The DetectionReport {PropertyName} is invalid.");
        }
    }
}
