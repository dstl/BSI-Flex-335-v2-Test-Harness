// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data; 

    public class VelocityTypeValidator : AbstractValidator<Registration.Types.VelocityType>
    {
        public VelocityTypeValidator()
        {
            RuleFor(x => x.EnuVelocityUnits).NotNull().NotEmpty().WithMessage("The VelocityType {PropertyName} is mandatory.");
            RuleFor(x => x.EnuVelocityUnits).SetValidator(new ENUVelocityUnitsValidator()).WithMessage("The VelocityType {PropertyName} is invalid.");

            RuleFor(x => new { x.LocationDatum, x.HasLocationDatum, x.RangeBearingDatum, x.HasRangeBearingDatum }).Must(x =>
            {
                bool result = false;
                if (x.HasLocationDatum || x.HasRangeBearingDatum)
                {
                    if (x.HasLocationDatum)
                    {
                        result = x.LocationDatum != Sapient.Common.LocationDatum.Unspecified;
                    }
                    if (x.HasRangeBearingDatum)
                    {
                        result = x.RangeBearingDatum != Sapient.Common.RangeBearingDatum.Unspecified;
                    }
                }
                if (x.HasLocationDatum && x.HasRangeBearingDatum) 
                {
                    result = false;
                }
                return result;  
            }).WithMessage("The VelocityType {PropertyName} is mandatory. Must have a location or bearing.");
        }
    }
}