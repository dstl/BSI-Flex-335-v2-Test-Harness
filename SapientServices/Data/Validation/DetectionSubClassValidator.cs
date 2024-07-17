// Crown-owned copyright, 2021-2024
using FluentValidation;

namespace SapientServices.Data.Validation
{
    public class DetectionSubClassValidator : AbstractValidator<Sapient.Data.DetectionReport.Types.SubClass>
    {
        public DetectionSubClassValidator() 
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().WithMessage("The Detection SubClass {PropertyName} is mandatory.");
            RuleFor(x => x.Confidence).Must(CommonValidator.BeAValidScalar).WithMessage("The Detection SubClass {PropertyName} is invalid.");
            RuleFor(x => x.Level).NotNull().WithMessage("The Detection SubClass {PropertyName} is mandatory.");
            RuleFor(x => x.HasLevel).Equal(true).WithMessage("The Detection SubClass {PropertyName} is mandatory.");
            RuleFor(x => x.SubClass_).Must((c) =>
            {
                bool result = true;
                if (c != null && c.Count > 0)
                {
                    foreach (Sapient.Data.DetectionReport.Types.SubClass subClass in c)
                    {
                        var validator = new DetectionSubClassValidator();
                        var validatorResults = validator.Validate(subClass);
                        if (!validatorResults.IsValid)
                        {
                            result = false;
                            break;
                        }
                    }
                }
                return result;
            }).WithMessage("The DetectionSubClass {PropertyName} is invalid.");
        }
    }
}
