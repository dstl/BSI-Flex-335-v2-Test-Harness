// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class BehaviourValidator : AbstractValidator<DetectionReport.Types.Behaviour>
    {
        public BehaviourValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The Detection Behaviour {PropertyName} is mandatory.");
            RuleFor(x => x.Type).Must(BeAValidBehaviourType).WithMessage("The Detection Behaviour {PropertyName} is invalid.");
            RuleFor(x => x.Confidence).Must(CommonValidator.BeAValidScalar).WithMessage("The Detection Behaviour {PropertyName} is invalid.");
        }

        private static bool BeAValidBehaviourType(string behaviourType)
        {
            switch (behaviourType.ToLower())
            {
                case "walking":
                case "running":
                case "crawling":
                case "climbing":
                case "digging":
                case "throwing":
                case "loitering":
                case "active":
                case "passive":
                case "other":
                    return true;
                default:
                    return false;
            }
        }
    }
}
