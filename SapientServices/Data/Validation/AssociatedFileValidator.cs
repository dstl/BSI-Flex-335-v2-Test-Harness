// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;

    public class AssociatedFileValidator : AbstractValidator<AssociatedFile>
    {
        public AssociatedFileValidator()
        {
            RuleFor(x => x.Type).NotNull().NotEmpty().WithMessage("The AssociatedFile {PropertyName} is mandatory. Type of file e.g. image.");
            RuleFor(x => x.Url).NotNull().NotEmpty().WithMessage("The AssociatedFile {PropertyName} is mandatory. URL to the media.");
        }
    }
}