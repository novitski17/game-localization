using GameLocalization.Core.Errors;
using FluentValidation.Results;

namespace GameLocalization.Core.Validation
{
    public static class ValidationErrorsHelper
    {
        public static ValidationAppError ToValidationAppError(ValidationResult result)
        {
            var dict = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, 
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return new ValidationAppError(dict);
        }
    }
}
