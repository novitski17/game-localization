using FluentValidation;

namespace GameLocalization.Core.Validation.Common
{
    public static class CoreRuleExtensions
    {
        public static IRuleBuilderOptions<T, string> CoreEmail<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = CoreValidationConst.EmailMaxLength)
            => rule
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.")
                .MaximumLength(maxLength)
                .WithMessage($"Email must be at most {maxLength} chars.");

        public static IRuleBuilderOptions<T, string> CorePassword<T>(
            this IRuleBuilder<T, string> rule,
            int minLength = CoreValidationConst.PasswordMinLength,
            int maxLength = CoreValidationConst.PasswordMaxLength)
            => rule
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(minLength).WithMessage($"Password must be at least {minLength} characters.")
                .MaximumLength(maxLength).WithMessage($"Password must be at most {maxLength} chars.");

        public static IRuleBuilderOptions<T, string> CoreLanguageCode<T>(
            this IRuleBuilder<T, string> rule,
            int minLength = CoreValidationConst.LanguageCodeMinLength,
            int maxLength = CoreValidationConst.LanguageCodeMaxLength,
            string pattern = CoreValidationConst.LanguageCodePattern)
            => rule
                .NotEmpty().WithMessage("Code is required.")
                .MinimumLength(minLength).WithMessage($"Code must be at least {minLength} chars.")
                .MaximumLength(maxLength).WithMessage($"Code must be at most {maxLength} chars.")
                .Matches(pattern).WithMessage("Code must contain only letters and hyphen.");

        public static IRuleBuilderOptions<T, string> CoreLanguageName<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = CoreValidationConst.LanguageNameMaxLength)
            => rule
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(maxLength).WithMessage($"Name must be at most {maxLength} chars.");

        public static IRuleBuilderOptions<T, string> CoreKeyCode<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = CoreValidationConst.KeyMaxLength,
            string pattern = CoreValidationConst.KeyCodePattern)
            => rule
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(maxLength).WithMessage($"Key must be at most {maxLength} chars.")
                .Matches(pattern).WithMessage("Key may contain letters, digits, '.', '-', '_',  only.");

        public static IRuleBuilderOptions<T, string?> CoreSearch<T>(
            this IRuleBuilder<T, string?> rule,
            int maxLength = CoreValidationConst.SearchMaxLength)
            => rule
                .Must(v => v == null || v.Trim().Length <= maxLength)
                .WithMessage($"Search must be at most {maxLength} chars.");

        public static IRuleBuilderOptions<T, int> CorePage<T>(
            this IRuleBuilder<T, int> rule,
            int min = CoreValidationConst.PageMin)
            => rule
                .GreaterThanOrEqualTo(min)
                .WithMessage($"Page must be greater than or equal to {min}.");

        public static IRuleBuilderOptions<T, int> CorePageSize<T>(
            this IRuleBuilder<T, int> rule,
            int min = CoreValidationConst.PageSizeMin,
            int max = CoreValidationConst.PageSizeMax)
            => rule
                .InclusiveBetween(min, max)
                .WithMessage($"PageSize must be between {min} and {max}.");

        public static IRuleBuilderOptions<T, string?> CoreTranslationValue<T>(
            this IRuleBuilder<T, string?> rule,
            int maxLength = CoreValidationConst.TranslationValueMaxLength)
            => rule
                .NotNull().WithMessage("Value is required.")
                .MaximumLength(maxLength)
                .WithMessage($"Value must be at most {maxLength} chars.");
    }
}

