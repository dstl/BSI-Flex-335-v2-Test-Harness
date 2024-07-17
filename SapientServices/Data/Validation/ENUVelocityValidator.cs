// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;

    public class ENUVelocityValidator : AbstractValidator<Sapient.Common.ENUVelocity>
    {
        public ENUVelocityValidator()
        {
            RuleFor(x => x.EastRate).NotNull().NotEmpty().WithMessage("The ENUVelocity {PropertyName} is mandatory. Velocity in the east-axis (x).");
            RuleFor(x => x.NorthRate).NotNull().NotEmpty().WithMessage("The ENUVelocity {PropertyName} is mandatory. Velocity in the north-axis (y)");
        }
    }
}
