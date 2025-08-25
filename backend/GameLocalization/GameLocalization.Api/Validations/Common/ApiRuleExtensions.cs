using FluentValidation;

namespace GameLocalization.Api.Validations.Common
{
    public static class ApiRuleExtensions
    {
        public static IRuleBuilderOptions<T, string> ApiLanguageName<T>(
        this IRuleBuilder<T, string> rule,
        int maxLength = ApiValidationConst.LanguageNameMaxLength)
        => rule
            .NotEmpty().WithMessage("Value is required.")
            .MaximumLength(maxLength).WithMessage($"Value must be at most {maxLength} chars.");

        public static IRuleBuilderOptions<T, string> ApiLanguageCode<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = ApiValidationConst.LanguageCodeMaxLength)
            => rule
                .NotEmpty().WithMessage("Language code is required.")
                .MaximumLength(maxLength).WithMessage($"Code must be at most {maxLength} chars.")
                .Matches(ApiValidationConst.LanguageCodePattern)
                .WithMessage("Code must contain only letters and hyphen.");

        public static IRuleBuilderOptions<T, string> ApiKeyCode<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = ApiValidationConst.KeyMaxLength)
            => rule
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(maxLength).WithMessage($"Key must be at most {maxLength} chars.")
                .Matches(ApiValidationConst.KeyCodePattern)
                .WithMessage("Key may contain letters, digits, '.', '-', '_',  only.");

        public static IRuleBuilderOptions<T, string?> ApiSearch<T>(
            this IRuleBuilder<T, string?> rule,
            int maxLength = ApiValidationConst.SearchMaxLength)
            => rule
                .Must(v => v == null || v.Trim().Length <= maxLength)
                .WithMessage($"Search must be at most {maxLength} chars.")
                .Must(v => v == null || (!v.Contains('%') && !v.Contains('_')))
                .WithMessage("Search must not contain '%' or '_' characters.");

        public static IRuleBuilderOptions<T, string> ApiEmail<T>(
            this IRuleBuilder<T, string> rule)
            => rule
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.");

        public static IRuleBuilderOptions<T, string> ApiPassword<T>(
            this IRuleBuilder<T, string> rule,
            int minLength = ApiValidationConst.PasswordMinLength)
            => rule
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(minLength)
                .WithMessage($"Password must be at least {minLength} characters.");
    }
}
