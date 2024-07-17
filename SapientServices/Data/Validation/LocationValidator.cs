// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;

    public class LocationValidator : AbstractValidator<Location>
    {
        public LocationValidator()
        {
            RuleFor(x => x.X).NotNull().WithMessage("The Location {PropertyName} is mandatory. Eastings up to 7 decimal places.");
            RuleFor(x => x.Y).NotNull().WithMessage("The Location {PropertyName} is mandatory. Northings up to 7 decimal places.");
            RuleFor(x => x.HasX).Equal(true).WithMessage("The Location X is mandatory. Eastings up to 7 decimal places.");
            RuleFor(x => x.HasY).Equal(true).WithMessage("The Location Y is mandatory. Northings up to 7 decimal places.");
            RuleFor(x => x.CoordinateSystem).NotNull().IsInEnum().NotEqual(LocationCoordinateSystem.Unspecified).WithMessage("The Location {PropertyName} is mandatory. Need the coordinate system used.");
            RuleFor(x => x.Datum).NotNull().IsInEnum().NotEqual(LocationDatum.Unspecified).WithMessage("The Location {PropertyName} is mandatory. Needs the datum used.");
        }
    }
}
