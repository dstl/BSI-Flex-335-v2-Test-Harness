// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class ErrorValidator : AbstractValidator<Error>
    {
        public ErrorValidator()
        {
            RuleFor(x => x.Packet).NotNull().NotEmpty().WithMessage("The Error {PropertyName} is mandatory.");
            RuleFor(x => x.ErrorMessage).NotNull().NotEmpty().Must((c) => { return c.Count > 0; }).WithMessage("The Error {PropertyName} is mandatory.");
        }
    }
}