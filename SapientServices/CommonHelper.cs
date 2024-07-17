// Crown-owned copyright, 2021-2024
using FluentValidation.Results;
using Google.Protobuf.WellKnownTypes;
using Sapient.Data;

namespace SapientServices
{
    /// <summary>
    /// The common helper.
    /// </summary>
    public static class CommonHelper
    {
        /// <summary>
        /// Tos the error string.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns>A string.</returns>
        public static string ToErrorString(this List<ValidationFailure> errors)
        {
            if (errors != null && errors.Any())
            {
                return string.Join(",\n", errors.Select(e => $"Field : {e.PropertyName}, Error : {e.ErrorMessage}"));
            }

            return string.Empty;
        }
    }
}