// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class SignalValidator : AbstractValidator<DetectionReport.Types.Signal>
    {
        public SignalValidator()
        {
            RuleFor(x => x.Amplitude).NotNull().NotEmpty().WithMessage("The Signal {PropertyName} is mandatory.");
            RuleFor(x => x.CentreFrequency).NotNull().NotEmpty().WithMessage("The Signal {PropertyName} is mandatory.");
        }
    }
}
