// Crown-owned copyright, 2021-2024
using FluentValidation;

namespace SapientServices.Data.Validation
{
    public class LocationOrRangeBearingValidator : AbstractValidator<Sapient.Common.LocationOrRangeBearing>
    {
        public LocationOrRangeBearingValidator()
        {
            RuleFor(x => x.LocationList)
                .NotNull()
                .When(x => x.FovOneofCase == Sapient.Common.LocationOrRangeBearing.FovOneofOneofCase.LocationList)
                .WithMessage("The LocationOrRangeBearing {PropertyName} is invalid.");

            RuleFor(x => x.LocationList)
                .SetValidator(new LocationListValidator())
                .When(x => x.FovOneofCase == Sapient.Common.LocationOrRangeBearing.FovOneofOneofCase.LocationList)
                .WithMessage("The LocationOrRangeBearing {PropertyName} is invalid.");

            RuleFor(x => x.RangeBearing)
                .NotNull()
                .When(x => x.FovOneofCase == Sapient.Common.LocationOrRangeBearing.FovOneofOneofCase.RangeBearing)
                .WithMessage("The LocationOrRangeBearing {PropertyName} is invalid.");

            RuleFor(x => x.RangeBearing)
                .SetValidator(new RangeBearingConeValidator())
                .When(x => x.FovOneofCase == Sapient.Common.LocationOrRangeBearing.FovOneofOneofCase.RangeBearing)
                .WithMessage("The LocationOrRangeBearing {PropertyName} is invalid.");
        }
    }
}
