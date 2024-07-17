// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class PerformanceValueValidator : AbstractValidator<Registration.Types.PerformanceValue>
    {
        public PerformanceValueValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The PerformanceValue {PropertyName} is mandatory.");
            RuleFor(x => x.Units).NotEmpty().NotNull().WithMessage("The PerformanceValue {PropertyName} is mandatory.");
            RuleFor(x => x.UnitValue).NotEmpty().NotNull().WithMessage("The PerformanceValue {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The PerformanceValue Type is mandatory.");
            RuleFor(x => x.HasUnits).Equal(true).WithMessage("The PerformanceValue Units is mandatory.");
            RuleFor(x => x.HasUnitValue).Equal(true).WithMessage("The PerformanceValue UnitValue is mandatory.");
        }
    }
}