// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class RegistrationAckValidator : AbstractValidator<RegistrationAck>
    {
        public RegistrationAckValidator()
        {
            RuleFor(x => x.HasAcceptance).Equal(true).WithMessage("The RegistrationAck Acceptance is mandatory. This field shall take a value of FALSE if the registration is rejected and a value of TRUE if the registration is accepted.");
        }
    }
}
