// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class ClassDefinitionValidator : AbstractValidator<Registration.Types.ClassDefinition>
    {
        public ClassDefinitionValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The ClassDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The ClassDefinition Type is mandatory.");
            RuleForEach(x => x.SubClass).SetValidator(new SubClassValidator()).WithMessage("The ClassDefinition {PropertyName} is invalid.");
        }
    }
}