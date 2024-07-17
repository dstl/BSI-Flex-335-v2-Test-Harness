// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Common;

    internal class AssociatedDetectionValidator : AbstractValidator<AssociatedDetection>
    {
        public AssociatedDetectionValidator()
        {
            RuleFor(x => x.NodeId).NotNull().NotEmpty().IsValidUuid().WithMessage("The AssociatedDetection {PropertyName} is mandatory. Unique identifier of fusion node that was the source of the detection.");
            RuleFor(x => x.ObjectId).NotNull().NotEmpty().IsValidUlid().WithMessage("The AssociatedDetection {PropertyName} is mandatory. Unique identifier of the detection.");
            RuleFor(x => x.AssociationType).NotNull().IsInEnum();
        }
    }
}