using FluentValidation;

namespace GameLocalization.Core.Validation.Languages
{
    public static class LanguageValidationRules
    {
        public static IRuleBuilderOptions<T, string> CodeRules<T>(this IRuleBuilder<T, string> r) =>
            r.NotEmpty().WithMessage("Code is required.")
                .Length(2, 5).WithMessage("Code must be 2..5 characters.")
                .Matches("^[a-zA-Z-]+$").WithMessage("Code must contain only letters and hyphen.");

        public static IRuleBuilderOptions<T, string> NameRules<T>(this IRuleBuilder<T, string> r) =>
            r.NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name is too long.");
    }
}
