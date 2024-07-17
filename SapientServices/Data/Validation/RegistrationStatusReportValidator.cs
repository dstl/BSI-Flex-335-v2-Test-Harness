// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;
    using static Sapient.Data.Registration.Types;

    public class RegistrationStatusReportValidator : AbstractValidator<Registration.Types.StatusReport>
    {
        public RegistrationStatusReportValidator()
        {
            RuleFor(x => x.Category).NotNull().NotEmpty().IsInEnum().NotEqual(StatusReportCategory.Unspecified).WithMessage("The Registration.StatusReport {PropertyName} is mandatory. The category of field to report.");
            RuleFor(x => x.HasCategory).Equal(true).WithMessage("The Registration.StatusReport Category is mandatory. The category of field to report.");
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The Registration.StatusReport Type is mandatory. The type or name of the information being provided.");
        }
    }
}
