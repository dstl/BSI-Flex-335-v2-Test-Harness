// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;

    public class RangeBearingValidator : AbstractValidator<RangeBearing>
    {
        public RangeBearingValidator()
        {
            RuleFor(x => x.CoordinateSystem).NotNull().IsInEnum().NotEqual(RangeBearingCoordinateSystem.Unspecified).WithMessage("The RangeBearing {PropertyName} is mandatory. Needs the coordinate system used.");
            RuleFor(x => x.Datum).NotNull().IsInEnum().NotEqual(RangeBearingDatum.Unspecified).WithMessage("The RangeBearing {PropertyName} is mandatory. Needs the datum used");
        }
    }

}
