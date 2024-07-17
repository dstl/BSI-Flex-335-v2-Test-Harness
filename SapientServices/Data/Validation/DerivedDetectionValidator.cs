// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class DerivedDetectionValidator : AbstractValidator<DetectionReport.Types.DerivedDetection>
    {
        public DerivedDetectionValidator()
        {
            RuleFor(x => x.Timestamp).NotNull().NotEmpty().WithMessage("The DerivedDetection {PropertyName} is mandatory.");
            RuleFor(x => x.NodeId).NotNull().NotEmpty().IsValidUuid().WithMessage("The DerivedDetection {PropertyName} is mandatory.");
            RuleFor(x => x.ObjectId).NotNull().NotEmpty().IsValidUlid().WithMessage("The DerivedDetection {PropertyName} is mandatory.");
        }
    }
}
