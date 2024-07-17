// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class StatusReportValidator : AbstractValidator<StatusReport>
    {
        public StatusReportValidator()
        {
            RuleFor(r => r.ReportId).NotNull().NotEmpty().IsValidUlid().WithMessage("The StatusDefinition {PropertyName} is mandatory.");
            RuleFor(r => r.HasReportId).Equal(true).WithMessage("The StatusDefinition ReportId is mandatory.");
            RuleFor(r => r.System).NotNull().IsInEnum().NotEqual(StatusReport.Types.System.Unspecified).WithMessage("The StatusDefinition {PropertyName} is mandatory.");
            RuleFor(r => r.HasSystem).Equal(true).WithMessage("The StatusDefinition System is mandatory.");
            RuleFor(r => r.Info).NotNull().IsInEnum().NotEqual(StatusReport.Types.Info.Unspecified).WithMessage("The StatusDefinition {PropertyName} is mandatory.");
            RuleFor(r => r.HasInfo).Equal(true).WithMessage("The StatusDefinition Info is mandatory.");
            RuleFor(r => r.ActiveTaskId).IsValidUlid().WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleFor(r => r.Mode).NotNull().NotEmpty().WithMessage("The StatusDefinition {PropertyName} is mandatory.");
            RuleFor(r => r.HasMode).Equal(true).WithMessage("The StatusDefinition Mode is mandatory.");
            RuleFor(r => r.Power).SetValidator(new PowerValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleFor(r => r.NodeLocation).SetValidator(new LocationValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleFor(r => r.FieldOfView).SetValidator(new LocationOrRangeBearingValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleForEach(r => r.Coverage).SetValidator(new LocationOrRangeBearingValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleForEach(r => r.Obscuration).SetValidator(new LocationOrRangeBearingValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
            RuleForEach(r => r.Status).SetValidator(new StatusReportStatusValidator()).WithMessage("The StatusDefinition {PropertyName} is invalid.");
        }
    }
}
