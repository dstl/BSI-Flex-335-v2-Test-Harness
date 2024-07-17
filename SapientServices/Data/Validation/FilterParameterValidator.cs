// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class FilterParameterValidator : AbstractValidator<Registration.Types.FilterParameter>
    { 
        public FilterParameterValidator()
        {
            RuleFor(x => x.Parameter).NotEmpty().NotNull().WithMessage("The FilterParameter {PropertyName} is mandatory.");
            RuleFor(x => x.HasParameter).Equal(true).WithMessage("The FilterParameter Parameter is mandatory.");
            RuleFor(x => x.Operators).NotNull().NotEmpty().WithMessage("The FilterParameter {PropertyName} is mandatory.");
            RuleForEach(x => x.Operators).NotNull().SetValidator(new OperatorValidator()).WithMessage("The FilterParameter Operator is invalid.");
        }
    }
}