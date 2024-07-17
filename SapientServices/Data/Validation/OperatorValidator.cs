// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class OperatorValidator : AbstractValidator<Operator>
    {
        public OperatorValidator()
        {
            RuleFor(x => x).NotEqual(Operator.Unspecified).WithMessage("The FilterParameter Operator is mandatory.");
        }        
    }
}