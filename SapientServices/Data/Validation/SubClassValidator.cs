// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;
    using static Sapient.Data.Registration.Types;

    public class SubClassValidator : AbstractValidator<Registration.Types.SubClass>
    {
        public SubClassValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The SubClass {PropertyName} is mandatory.");
            RuleFor(x => x.Level).NotNull().WithMessage("The SubClass {PropertyName} is mandatory."); 
            RuleFor(x => x.HasType).Equal(true).WithMessage("The SubClass Type is mandatory.");
            RuleFor(x => x.HasLevel).Equal(true).WithMessage("The SubClass Level is mandatory.");
            RuleFor(x => x.SubClass_).Must((c) =>
            {
                bool result = true;
                if (c != null && c.Count > 0)
                {
                    foreach (SubClass subClass in c)
                    {
                        var validator = new SubClassValidator();
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