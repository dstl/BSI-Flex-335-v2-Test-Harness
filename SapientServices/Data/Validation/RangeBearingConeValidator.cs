// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;

    public class RangeBearingConeValidator : AbstractValidator<RangeBearingCone>
    {
        public RangeBearingConeValidator()
        {
            RuleFor(x => x.CoordinateSystem).NotNull().IsInEnum().NotEqual(RangeBearingCoordinateSystem.Unspecified).WithMessage("The RangeBearingCone {PropertyName} is mandatory. Coordinate system used.");
            RuleFor(x => x.Datum).NotNull().IsInEnum().NotEqual(RangeBearingDatum.Unspecified).WithMessage("The RangeBearingCone {PropertyName} is mandatory. Datum used");
        }
    }
}
