// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class ExtensionSubclassValidator : AbstractValidator<Registration.Types.ExtensionSubclass>
    {
        public ExtensionSubclassValidator()
        {
            RuleFor(x => x.SubclassNamespace).NotEmpty().NotNull().WithMessage("The ExtensionSubclass {PropertyName} is mandatory.");
            RuleFor(x => x.HasSubclassNamespace).Equal(true).WithMessage("The ExtensionSubclass SubclassNamespace is mandatory.");
            RuleFor(x => x.SubclassName).NotEmpty().NotNull().WithMessage("The ExtensionSubclass {PropertyName} is mandatory.");
            RuleFor(x => x.HasSubclassName).Equal(true).WithMessage("The ExtensionSubclass SubclassName is mandatory.");
        }
    }
}