namespace GameLocalization.Core.Extensions
{
    public static class StringExtensions
    {
        public static string NormalizeCode(this string? value)
            => (value ?? string.Empty).Trim().ToLowerInvariant();
    }
}
