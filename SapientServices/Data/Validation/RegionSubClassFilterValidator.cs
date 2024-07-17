// Crown-owned copyright, 2021-2024
using FluentValidation;
using static Sapient.Data.Registration.Types;
using static Sapient.Data.Task.Types;

namespace SapientServices.Data.Validation
{
    public class RegionSubClassFilterValidator : AbstractValidator<SubClassFilter>
    {
        public RegionSubClassFilterValidator()
        {
            RuleFor(x => x.Parameter).NotEmpty().NotNull().WithMessage("The Region SubClassFilter {PropertyName} is mandatory.");
            RuleFor(x => x.Parameter).SetValidator(new TaskParameterValidator()).WithMessage("The Region SubClassFilter {PropertyName} is invalid.");
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The Region SubClassFilter {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The Region SubClassFilter Type is mandatory.");
            RuleFor(x => x.SubClassFilter_).Must((c) =>
            {
                bool result = true;
                if (c != null && c.Count > 0)
                {
                    foreach (SubClassFilter subClassFilter in c)
                    {
                        var validator = new RegionSubClassFilterValidator();
                        var validatorResults = validator.Validate(subClassFilter);
                        if (!validatorResults.IsValid)
                        {
                            result = false;
                            break;
                        }
                    }
                }
                return result;
            }).WithMessage("The Region SubClassFilter {PropertyName} is invalid.");

        }
    }
}
