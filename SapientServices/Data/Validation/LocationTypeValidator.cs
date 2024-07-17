// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;
    using Sapient.Data;

    public class LocationTypeValidator : AbstractValidator<Registration.Types.LocationType>
    {
        public LocationTypeValidator(bool CanOnlyBeLocation)
        {
            if (CanOnlyBeLocation)
            {
                RuleFor(x => x.LocationUnits).NotNull().NotEmpty().WithMessage("The LocationType {PropertyName} is mandatory.");
                RuleFor(x => x.LocationDatum).NotNull().NotEmpty().WithMessage("The LocationType {PropertyName} is mandatory.");
                RuleFor(x => x.HasLocationUnits).Equal(true).WithMessage("The LocationType LocationUnits is mandatory.");
                RuleFor(x => x.HasLocationDatum).Equal(true).WithMessage("The LocationType LocationDatum is mandatory.");
                RuleFor(x => x.LocationUnits).NotEqual(LocationCoordinateSystem.Unspecified).WithMessage("The LocationType {PropertyName} is mandatory.");
                RuleFor(x => x.LocationDatum).NotEqual(LocationDatum.Unspecified).WithMessage("The LocationType {PropertyName} is mandatory.");

            }
            else
            {
                RuleFor(x => new
                {
                    x.LocationUnits,
                    x.HasLocationUnits,
                    x.LocationDatum,
                    x.HasLocationDatum,
                    x.RangeBearingUnits,
                    x.HasRangeBearingUnits,
                    x.RangeBearingDatum,
                    x.HasRangeBearingDatum,
                    x.Zone,
                    x.HasZone,
                }).Must(x =>
                {
                    bool result = false;

                    if ((x.HasLocationUnits)
                    && (x.HasLocationDatum)
                    && (x.LocationUnits != LocationCoordinateSystem.Unspecified)
                    && (x.LocationDatum != LocationDatum.Unspecified)
                    && (!x.HasRangeBearingUnits)
                    && (!x.HasRangeBearingDatum))
                    {
                        result = true;
                    }

                    if ((!x.HasLocationUnits)
                    && (!x.HasLocationDatum)
                    && (x.HasRangeBearingUnits)
                    && (x.HasRangeBearingDatum)
                    && (x.RangeBearingUnits != RangeBearingCoordinateSystem.Unspecified)
                    && (x.RangeBearingDatum != RangeBearingDatum.Unspecified))
                    {
                        result = true;
                    }

                    return result;
                }).WithMessage("Must have either a location or bearing but not both.");

                RuleFor(x => new
                {
                    x.LocationUnits,
                    x.HasLocationUnits,
                    x.Zone,
                    x.HasZone,
                }).Must(x =>
                {
                    bool result = true;
                    if ((x.HasLocationUnits)
                    && (x.LocationUnits == LocationCoordinateSystem.UtmM)
                    && ((!x.HasZone) || (string.IsNullOrEmpty(x.Zone))))
                    {
                        result = false;
                    }
                    return result;
                }).WithMessage("If using UTM the location must have a UTM Zone .");

            }
        }
    }
}
