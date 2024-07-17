// Crown-owned copyright, 2021-2024
using FluentValidation;
using static Sapient.Data.Registration.Types;
using static Sapient.Data.Task.Types;

namespace SapientServices.Data.Validation
{
    public class RegionClassFilterValidator : AbstractValidator<ClassFilter>
    {
        public RegionClassFilterValidator()
        {
            RuleFor(x => x.Parameter).NotEmpty().NotNull().WithMessage("The Region ClassFilter {PropertyName} is mandatory.");
            RuleFor(x => x.Parameter).SetValidator(new TaskParameterValidator()).WithMessage("The Region ClassFilter {PropertyName} is invalid.");
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The Region ClassFilter {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The Region ClassFilter Type is mandatory.");
            RuleForEach(x => x.SubClassFilter).SetValidator(new RegionSubClassFilterValidator()).WithMessage("The Region ClassFilter {PropertyName} is invalid.");
        }
    }
}
