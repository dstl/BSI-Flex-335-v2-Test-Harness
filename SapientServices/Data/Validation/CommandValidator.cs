// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class CommandValidator : AbstractValidator<Registration.Types.Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Units).NotEmpty().NotNull().WithMessage("The CommandValidator {PropertyName} is mandatory.");
            RuleFor(x => x.HasUnits).Equal(true).WithMessage("The CommandValidator Units is mandatory.");
            RuleFor(x => x.CompletionTime).NotNull().NotEmpty().SetValidator(new DurationValidator()).WithMessage("The Location {PropertyName} is invalid.");
            RuleFor(x => x.Type).NotNull().NotEmpty().IsInEnum().NotEqual(Registration.Types.CommandType.Unspecified).WithMessage("The CommandValidator {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The CommandValidator Type is mandatory.");
        }
    }
}