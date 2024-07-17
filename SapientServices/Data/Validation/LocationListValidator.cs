// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;

    public class LocationListValidator : AbstractValidator<LocationList>
    {
        public LocationListValidator()
        {
            RuleFor(r => r.Locations)
                .NotNull()
                .NotEmpty()
                .WithMessage("The LocationList {PropertyName} is mandatory. A list of locations used to define a region or field of view.");
            RuleForEach(r => r.Locations)
                .SetValidator(new LocationValidator())
                .WithMessage("The LocationList {PropertyName} is invalid.");
        }
    }
}
