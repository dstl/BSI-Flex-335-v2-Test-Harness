// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class NodeDefinitionValidator : AbstractValidator<Registration.Types.NodeDefinition>
    {
        public NodeDefinitionValidator()
        {
            RuleFor(x => x.NodeType)
                .NotEmpty()
                .IsInEnum()              
                .Must(p => (int)p != 0)
                .WithMessage("The NodeDefinition {PropertyName} is mandatory. The generic type(s) of sensor or effector supported by the Node.");
        }
    }

}
