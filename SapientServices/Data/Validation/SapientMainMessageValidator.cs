// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    /// <summary>
    /// The sapient main message validator.
    /// </summary>
    public class SapientMainMessageValidator : AbstractValidator<SapientMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SapientMainMessageValidator"/> class.
        /// </summary>
        public SapientMainMessageValidator()
        {
            RuleFor(x => x.Timestamp).NotNull().WithMessage("The SapientMessage {PropertyName} is mandatory. UTC time of the content of the message.");
            RuleFor(x => x.NodeId).NotEmpty().IsValidUuid().WithMessage("The SapientMessage {PropertyName} is mandatory. The node ID of the sender of the message.");
            RuleFor(x => x.DestinationId).IsValidUuid().WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => new 
            { 
                x.Registration, 
                x.RegistrationAck, 
                x.StatusReport, 
                x.Alert, 
                x.AlertAck, 
                x.DetectionReport, 
                x.Task, 
                x.TaskAck, 
                x.Error 
            }).Must( x => 
                x.Registration != null || 
                x.RegistrationAck != null || 
                x.StatusReport != null || 
                x.Alert != null || 
                x.AlertAck != null || 
                x.DetectionReport != null || 
                x.Task != null || 
                x.TaskAck != null || 
                x.Error != null).WithMessage("The SapientMessage does not have a content.");
            RuleFor(x => x.Alert).SetValidator(new AlertValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.AlertAck).SetValidator(new AlertAckValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.DetectionReport).SetValidator(new DetectionReportValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.Registration).SetValidator(new RegistrationValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.RegistrationAck).SetValidator(new RegistrationAckValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.StatusReport).SetValidator(new StatusReportValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.Task).SetValidator(new TaskValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.TaskAck).SetValidator(new TaskAcknowledgementValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
            RuleFor(x => x.Error).SetValidator(new ErrorValidator()).WithMessage("The SapientMessage {PropertyName} is invalid.");
        }
    }
}
