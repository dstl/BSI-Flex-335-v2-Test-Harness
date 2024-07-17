// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class RegionTypeValidator : AbstractValidator<Registration.Types.RegionType>
    { 
        public RegionTypeValidator()
        {
            RuleFor(x => x).NotNull().NotEmpty().NotEqual(Registration.Types.RegionType.Unspecified).WithMessage("The RegionDefinition RegionType is mandatory.");
        }
    }
}