// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;
using static Sapient.Data.Task.Types;

namespace SapientServices.Data.Validation
{
    public class TaskCommandValidator : AbstractValidator<Command>
    {
        public TaskCommandValidator()
        {
            RuleFor(x => x.LookAt)
                .SetValidator(new LocationOrRangeBearingValidator());             
            RuleFor(x => x.Patrol)
                .SetValidator(new LocationListValidator());
            RuleFor(x => x.MoveTo)
                .SetValidator(new LocationListValidator());
            RuleFor(x => x.Follow)
                .SetValidator(new FollowObjectValidator());

            RuleFor(x => new
            {
                x.CommandCase,
                x.Request,
                x.DetectionThreshold,
                x.DetectionReportRate,
                x.ClassificationThreshold,
                x.ModeChange,
                x.LookAt,
                x.MoveTo,
                x.Patrol,
                x.Follow,
            }).Custom((c, context) =>
            {
                switch (c.CommandCase)
                {
                    case Command.CommandOneofCase.None:
                        context.AddFailure("The Task Command is mandatory.");
                        break;
                    case Command.CommandOneofCase.Request:
                        if (string.IsNullOrEmpty(c.Request))
                        {
                            context.AddFailure("The Task Command is mandatory. Missing Request.");
                        }
                        break;
                    case Command.CommandOneofCase.DetectionThreshold:
                        if (c.DetectionThreshold == DiscreteThreshold.Unspecified)
                        {
                            context.AddFailure("The Task Command is mandatory. DetectionThreshold unspecified.");
                        }
                        break;
                    case Command.CommandOneofCase.DetectionReportRate:
                        if (c.DetectionReportRate == DiscreteThreshold.Unspecified)
                        {
                            context.AddFailure("The Task Command is mandatory. DetectionReportRate unspecified.");
                        }
                        break;
                    case Command.CommandOneofCase.ClassificationThreshold:
                        if (c.ClassificationThreshold == DiscreteThreshold.Unspecified)
                        {
                            context.AddFailure("The Task Command is mandatory. ClassificationThreshold unspecified.");
                        }
                        break;
                    case Command.CommandOneofCase.ModeChange:
                        if (string.IsNullOrEmpty(c.ModeChange))
                        {
                            context.AddFailure("The Task Command is mandatory. Missing ModeChange.");
                        }
                        break;
                    case Command.CommandOneofCase.LookAt:
                        if (c.LookAt == null)
                        {
                            context.AddFailure("The Task Command is mandatory. Missing LookAt.");
                        }
                        break;
                    case Command.CommandOneofCase.MoveTo:
                        if ((c.MoveTo == null) || (c.MoveTo.Locations == null) || (c.MoveTo.Locations.Count == 0))
                        {
                            context.AddFailure("The Task Command MoveTo is mandatory. Missing MoveTo.");
                        }
                        break;
                    case Command.CommandOneofCase.Patrol:
                        if ((c.Patrol == null) || (c.Patrol.Locations == null) || (c.Patrol.Locations.Count == 0))
                        {
                            context.AddFailure("The Task Command is mandatory. Missing Patrol.");
                        }
                        break;
                    case Command.CommandOneofCase.Follow:
                        if (c.Follow == null)
                        {
                            context.AddFailure("The Task Command is mandatory. Missing Follow.");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(c.Follow.FollowObjectId))
                            {
                                context.AddFailure("The Task Command is mandatory. Missing Follow FollowObjectId.");
                            }
                            else
                            {
                                if (!CommonValidator.IsValidUlid(c.Follow.FollowObjectId))
                                {
                                    context.AddFailure("The Task Command is mandatory. The FollowObjectId is invalid.");
                                }
                            }
                        }
                        break;
                    default:
                        context.AddFailure("The Task Command is mandatory. Has an unknown command value.");
                        break;
                }
            });
        }
    }
}