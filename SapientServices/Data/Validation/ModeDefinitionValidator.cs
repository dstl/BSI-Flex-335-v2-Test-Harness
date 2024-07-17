// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class ModeDefinitionValidator : AbstractValidator<Registration.Types.ModeDefinition>
    {
        public ModeDefinitionValidator()
        {
            RuleFor(x => x.ModeName).NotEmpty().NotNull().WithMessage("The ModeDefinition {PropertyName} is mandatory. Name of mode.");
            RuleFor(x => x.HasModeName).Equal(true).WithMessage("The ModeDefinition ModeName is mandatory. Name of mode.");
            RuleFor(x => x.ModeType).NotEmpty().NotNull().IsInEnum().NotEqual(Registration.Types.ModeType.Unspecified).WithMessage("The ModeDefinition {PropertyName} is mandatory. Type of mode (permanent, temporary).");
            RuleFor(x => x.SettleTime).NotNull().WithMessage("The ModeDefinition {PropertyName} is mandatory. Time to settle to normal performance.");
            RuleFor(x => x.SettleTime).SetValidator(new DurationValidator()).WithMessage("The ModeDefinition {PropertyName} is invalid.");
            RuleFor(x => x.MaximumLatency).SetValidator(new DurationValidator()).WithMessage("The ModeDefinition {PropertyName} is invalid.");
            RuleFor(x => x.Duration).SetValidator(new DurationValidator()).WithMessage("The ModeDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.ModeParameter).SetValidator(new ModeParameterValidator()).WithMessage("The ModeDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.DetectionDefinition).NotNull().SetValidator(new DetectionDefinitionValidator()).WithMessage("The ModeDefinition {PropertyName} is invalid.");
            RuleFor(x => x.Task).NotEmpty().NotNull().WithMessage("The ModeDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.Task).SetValidator(new TaskDefinitionValidator()).WithMessage("The ModeDefinition {PropertyName} is invalid.");
        }
    }
}