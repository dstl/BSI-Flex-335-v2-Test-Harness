// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class AlertAckValidator : AbstractValidator<AlertAck>
    {
        public AlertAckValidator()
        {
            RuleFor(x => x.AlertId).NotEmpty().NotNull().IsValidUlid().WithMessage("The AlertAck {PropertyName} is mandatory.");
            RuleFor(x => x.AlertAckStatus).NotEmpty().IsInEnum().NotEqual(AlertAck.Types.AlertAckStatus.Unspecified).WithMessage("The AlertAck {PropertyName} is mandatory.");
        }
    }
}
