// Crown-owned copyright, 2021-2024
using FluentValidation;
using static Sapient.Data.Task.Types;

namespace SapientServices.Data.Validation
{
    public class TaskRegionValidator : AbstractValidator<Region>
    {
        public TaskRegionValidator()
        {
            RuleFor(x => x.Type).NotEmpty().NotNull().IsInEnum().NotEqual(RegionType.Unspecified).WithMessage("The TaskRegion {PropertyName} is mandatory.");
            RuleFor(x => x.HasType).Equal(true).WithMessage("The TaskRegion Type is mandatory.");
            RuleFor(x => x.RegionId).NotEmpty().NotNull().IsValidUlid().WithMessage("The TaskRegion {PropertyName} is mandatory.");
            RuleFor(x => x.HasRegionId).Equal(true).WithMessage("The TaskRegion Region Id is mandatory.");
            RuleFor(x => x.RegionName).NotEmpty().NotNull().WithMessage("The TaskRegion {PropertyName} is mandatory.");
            RuleFor(x => x.HasRegionName).Equal(true).WithMessage("The TaskRegion Region Name is mandatory.");
            RuleFor(x => x.RegionArea).NotEmpty().NotNull().SetValidator(new LocationOrRangeBearingValidator()).WithMessage("The TaskRegion {PropertyName} is invalid.");
            RuleForEach(x => x.ClassFilter).SetValidator(new RegionClassFilterValidator()).WithMessage("The TaskRegion {PropertyName} is invalid.");
            RuleForEach(x => x.BehaviourFilter).SetValidator(new TaskBehaviourFilterValidator()).WithMessage("The TaskRegion {PropertyName} is invalid.");
        }
    }
}
