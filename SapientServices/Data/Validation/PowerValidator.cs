// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;

namespace SapientServices.Data.Validation
{ 
    public class PowerValidator : AbstractValidator<StatusReport.Types.Power>
    {
        public PowerValidator()
        {
        }
    }
}
