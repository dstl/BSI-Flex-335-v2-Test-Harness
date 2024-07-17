// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;

    public class ENUVelocityUnitsValidator : AbstractValidator<ENUVelocityUnits> 
    {
        public ENUVelocityUnitsValidator() 
        {
            RuleFor(x => x.EastNorthRateUnits).NotNull().IsInEnum().NotEqual(SpeedUnits.Unspecified).WithMessage("The ENUVelocityUnits {PropertyName} is mandatory.");
        }
    }
}
