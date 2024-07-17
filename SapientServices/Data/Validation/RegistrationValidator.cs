// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{
    public class RegistrationValidator : AbstractValidator<Registration>
    {
        public RegistrationValidator() 
        {
            RuleFor(x => x.NodeDefinition).NotEmpty().Must(x => x.Any()).WithMessage("The Registration {PropertyName} is mandatory. Node type defines the broad category of node registering to allow the fusion node to handle the node appropriately.");
            RuleForEach(x => x.NodeDefinition).NotNull().SetValidator(new NodeDefinitionValidator()).WithMessage("The Registration {PropertyName} is invalid.");
            RuleFor(x => x.HasIcdVersion).Equal(true).WithMessage("The Registration IcdVersion is mandatory. ICD Version implemented by the node.");
            RuleFor(x => x.IcdVersion).NotEmpty().Equal("BSI Flex 335 v2.0").WithMessage("The Registration {PropertyName} is mandatory. ICD Version implemented by the node.");
            RuleFor(x => x.Capabilities).NotEmpty().Must(x => x.Any()).WithMessage("The Registration {PropertyName} is mandatory. List of node capabilities.");
            RuleForEach(x => x.Capabilities).SetValidator(new CapabilityValidator()).WithMessage("The Registration {PropertyName} is invalid.");
            RuleFor(x => x.StatusDefinition).NotNull().SetValidator(new StatusDefinitionValidator()).WithMessage("The Registration {PropertyName} is invalid.");
            RuleFor(x => x.ModeDefinition).NotEmpty().Must(x => x.Any()).WithMessage("The Registration {PropertyName} is mandatory.");
            RuleForEach(x => x.ModeDefinition).SetValidator(new ModeDefinitionValidator()).WithMessage("The Registration {PropertyName} is invalid.");
            RuleFor(x => x.ConfigData).NotEmpty().Must(x => x.Any()).WithMessage("The Registration {PropertyName} is mandatory. Configuration data lists information to manage nodes and their configuration status.");
            RuleForEach(x => x.ConfigData).SetValidator(new ConfigurationDataValidator()).WithMessage("The Registration {PropertyName} is invalid.");
            RuleForEach(x => x.DependentNodes).IsValidUuid().WithMessage("The Registration {PropertyName} is invalid.");
        }
    }
}
