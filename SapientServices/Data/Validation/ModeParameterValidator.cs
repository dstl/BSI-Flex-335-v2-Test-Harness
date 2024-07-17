// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class ModeParameterValidator : AbstractValidator<Registration.Types.ModeParameter>
    {
        public ModeParameterValidator()
        {
            RuleFor(x => x.Type).NotEmpty().WithMessage("The ModeParameter {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The ModeParameter Type is mandatory.");
            RuleFor(x => x.Value).NotEmpty().WithMessage("The ModeParameter {PropertyName} is mandatory.");
            RuleFor(x => x.HasValue).Equal(true).WithMessage("The ModeParameter Value is mandatory.");
        }
    }
}
