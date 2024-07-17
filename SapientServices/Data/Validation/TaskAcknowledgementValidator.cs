// Crown-owned copyright, 2021-2024
using FluentValidation;

namespace SapientServices.Data.Validation
{
    public class TaskAcknowledgementValidator : AbstractValidator<Sapient.Data.TaskAck>
    {
        public TaskAcknowledgementValidator()
        {
            RuleFor(t => t.TaskId).NotEmpty().NotNull().IsValidUlid().WithMessage("The TaskAcknowledgement {PropertyName} is mandatory.");
            RuleFor(t => t.TaskStatus).NotNull().IsInEnum().WithMessage("The TaskAcknowledgement {PropertyName} is mandatory.");
            RuleFor(t => t.TaskStatus).NotEqual(Sapient.Data.TaskAck.Types.TaskStatus.Unspecified).WithMessage("The TaskAcknowledgement {PropertyName} is mandatory.");
            RuleFor(t => t.AssociatedFile).SetValidator(new AssociatedFileValidator()).WithMessage("The TaskAcknowledgement {PropertyName} is invalid.");
        }
    }
}
