// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using System.Collections.Generic;
    using static Sapient.Data.Registration.Types;

    public class ConfigurationDataValidator : AbstractValidator<ConfigurationData>
    {
        public ConfigurationDataValidator()
        {
            RuleFor(x => x.Manufacturer).NotNull().NotEmpty().WithMessage("The ConfigurationData {PropertyName} is mandatory. The name of the manufacturer of the node.");
            RuleFor(x => x.Model).NotNull().NotEmpty().WithMessage("The ConfigurationData {PropertyName} is mandatory. The name of the product of the node.");
            RuleFor(x => x.SubComponents).Must((c) =>
            {
                bool result = true;
                if (c != null && c.Count > 0)
                {
                    foreach (ConfigurationData configurationData in c)
                    {
                        var validator = new ConfigurationDataValidator();
                        var validatorResults = validator.Validate(configurationData);
                        if (!validatorResults.IsValid)
                        {
                            result = false;
                            break;
                        }
                    }
                }
                return result;
            }).WithMessage("The ConfigurationData {PropertyName} is invalid.");
        }
    }
}
