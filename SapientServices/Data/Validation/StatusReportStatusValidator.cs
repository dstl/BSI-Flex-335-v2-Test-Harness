// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class StatusReportStatusValidator : AbstractValidator<StatusReport.Types.Status>
    {
        public StatusReportStatusValidator() 
        {
            RuleFor(s => s.StatusType).NotNull().NotEmpty().WithMessage("The StatusReport.Status Type is mandatory.");
            RuleFor(s => s.StatusType).NotEqual(StatusReport.Types.StatusType.Unspecified).WithMessage("The StatusReport.Status Type is mandatory.");
            RuleFor(s => s.HasStatusType).Equal(true).WithMessage("The StatusReport,Status Type is mandatory.");
        }
    }
}
