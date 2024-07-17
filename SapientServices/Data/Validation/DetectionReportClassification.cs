// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class DetectionReportClassificationValidator : AbstractValidator<DetectionReport.Types.DetectionReportClassification>
    {
        public DetectionReportClassificationValidator()
        {
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("The ClassValidator {PropertyName} is mandatory.");
            RuleForEach(x => x.SubClass).SetValidator(new DetectionSubClassValidator()).WithMessage("The ClassValidator {PropertyName} is invalid.");
        }
    }
}
