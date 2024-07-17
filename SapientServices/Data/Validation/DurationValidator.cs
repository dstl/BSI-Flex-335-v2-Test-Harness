// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;
    using static Sapient.Data.Registration.Types;

    public class DurationValidator : AbstractValidator<Registration.Types.Duration>
    {
        public DurationValidator()
        {
            RuleFor(x => x.Units).NotNull().NotEmpty().IsInEnum().NotEqual(TimeUnits.Unspecified).WithMessage("The Duration {PropertyName} is mandatory. A description of the units.");
            RuleFor(x => x.HasUnits).Equal(true).WithMessage("The Duration Units is mandatory. A description of the units.");
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage("The Duration {PropertyName} is mandatory.");
            RuleFor(x => x.HasValue).Equal(true).WithMessage("The Duration Value is mandatory.");
        }
    }
}
