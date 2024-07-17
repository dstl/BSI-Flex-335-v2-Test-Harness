// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class TaxonomyDockDefinitionValidator : AbstractValidator<Registration.Types.TaxonomyDockDefinition>
    {
        public TaxonomyDockDefinitionValidator()
        {
            RuleFor(x => x.DockClass).NotEmpty().NotNull().WithMessage("The TaxonomyDockDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasDockClass).Equal(true).WithMessage("The TaxonomyDockDefinition DockClass is mandatory.");
            RuleForEach(x => x.ExtensionSubclass).SetValidator(new ExtensionSubclassValidator()).WithMessage("The TaxonomyDockDefinition {PropertyName} is invalid.");
        }
    }
}