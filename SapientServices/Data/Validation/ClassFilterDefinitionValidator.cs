// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class ClassFilterDefinitionValidator : AbstractValidator<Registration.Types.ClassFilterDefinition>
    {
        public ClassFilterDefinitionValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The ClassFilterDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The ClassFilterDefinition Type is mandatory.");
            RuleForEach(x => x.FilterParameter).SetValidator(new FilterParameterValidator()).WithMessage("The ClassFilterDefinition {PropertyName} is invalid.");
            RuleForEach(x => x.SubClassDefinition).SetValidator(new SubClassFilterDefinitionValidator()).WithMessage("The ClassFilterDefinition {PropertyName} is invalid.");
        }
    }
}