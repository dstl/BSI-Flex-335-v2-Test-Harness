// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;
    using static Sapient.Data.Registration.Types;

    public class SubClassFilterDefinitionValidator : AbstractValidator<Registration.Types.SubClassFilterDefinition>
    {
        public SubClassFilterDefinitionValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The SubClassFilterDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The SubClassFilterDefinition Type is mandatory.");
            RuleFor(x => x.Level).NotNull().WithMessage("The SubClassFilterDefinition {PropertyName} is mandatory.");
            RuleFor(x => x.HasLevel).Equal(true).WithMessage("The SubClassFilterDefinition Level is mandatory.");
            RuleForEach(x => x.FilterParameter).SetValidator(new FilterParameterValidator());
            RuleFor(x => x.SubClassDefinition).Must((c) =>
            {
                bool result = true;
                if (c != null && c.Count > 0)
                {
                    foreach (SubClassFilterDefinition subClass in c)
                    {
                        var validator = new SubClassFilterDefinitionValidator();
                        var validatorResults = validator.Validate(subClass);
                        if (!validatorResults.IsValid)
                        {
                            result = false;
                            break;
                        }
                    }
                }
                return result;
            }).WithMessage("The SubClass {PropertyName} is invalid.");
        }
    }
}