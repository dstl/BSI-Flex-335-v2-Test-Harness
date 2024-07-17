// Crown-owned copyright, 2021-2024
namespace SapientServices.Data.Validation
{
    using FluentValidation;
    using Sapient.Data;

    public class FollowObjectValidator : AbstractValidator<FollowObject>
    {
        public FollowObjectValidator()
        {
            RuleFor(x => x.FollowObjectId)
                .NotEmpty()
                .NotNull()
                .IsValidUlid()
                .WithMessage("The FollowObject {PropertyName} is mandatory. This is the object_id (ULID) of the object to follow.");
        }
    }
}