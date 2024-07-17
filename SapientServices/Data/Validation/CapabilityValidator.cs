// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class CapabilityValidator : AbstractValidator<Registration.Types.Capability>
    { 
        public CapabilityValidator()
        {
            RuleFor(x => x.HasCategory).Equal(true).WithMessage("The Capability Category is mandatory. The category of field to report.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The Capability Type is mandatory. Description of the capabilit.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("The Capability {PropertyName} is mandatory. The category of field to report.");
            RuleFor(x => x.Type).NotEmpty().WithMessage("The Capability {PropertyName} is mandatory. Description of the capabilit.");
        }
    }
}
