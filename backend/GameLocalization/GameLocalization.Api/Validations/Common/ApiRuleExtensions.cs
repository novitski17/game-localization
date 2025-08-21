using System.Text.RegularExpressions;
using FluentValidation;

namespace GameLocalization.Api.Validations.Common
{
    public static class ApiRuleExtensions
    {
        public static IRuleBuilderOptions<T, string> ApiShortString<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = 50)
        {
            return rule
                .NotEmpty().WithMessage("Value is required.")
                .MaximumLength(maxLength).WithMessage($"Value must be at most {maxLength} chars.");
        }
        
        public static IRuleBuilderOptions<T, string> ApiLanguageCodeLoose<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = 10)
        {
            return rule
                .NotEmpty().WithMessage("Language code is required.")
                .MaximumLength(maxLength).WithMessage($"Code must be at most {maxLength} chars.")
                .Must(v => Regex.IsMatch(v, @"^[A-Za-z-]+$"))
                .WithMessage("Code must contain only letters and hyphen.");
        }

        public static IRuleBuilderOptions<T, string> ApiKeyCode<T>(
            this IRuleBuilder<T, string> rule,
            int maxLength = 150,
            string pattern = @"^[A-Za-z0-9._-]+$")
        {
            return rule
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(maxLength).WithMessage($"Key must be at most {maxLength} chars.")
                .Matches(pattern).WithMessage("Key may contain letters, digits, '.', '-', '_',  only.");
        }

        public static IRuleBuilderOptions<T, string?> ApiSearch<T>(
            this IRuleBuilder<T, string?> rule,
            int maxLength = 150)
        {
            return rule
                .Must(v => v == null || v.Trim().Length <= maxLength)
                .WithMessage($"Search must be at most {maxLength} chars.")
                .Must(v => v == null || (!v.Contains('%') && !v.Contains('_')))
                .WithMessage("Search must not contain '%' or '_' characters.");
        }
    }
}
