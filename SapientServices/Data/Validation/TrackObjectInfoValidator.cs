// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class TrackObjectInfoValidator : AbstractValidator<DetectionReport.Types.TrackObjectInfo>
    {
        public TrackObjectInfoValidator() 
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The DetectionReport {PropertyName} is mandatory.");
            RuleFor(x => x.Value).NotEmpty().NotNull().WithMessage("The DetectionReport {PropertyName} is mandatory.");
        }
    }
}
