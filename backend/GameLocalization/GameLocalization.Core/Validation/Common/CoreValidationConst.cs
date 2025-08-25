namespace GameLocalization.Core.Validation.Common
{
    public static class CoreValidationConst
    {
        public const int EmailMaxLength = 256;
        public const int PasswordMinLength = 6;
        public const int PasswordMaxLength = 128;

        public const int LanguageCodeMaxLength = 5;
        public const int LanguageCodeMinLength = 2;
        public const int LanguageNameMaxLength = 50;

        public const int KeyMaxLength = 150;

        public const int TranslationValueMaxLength = 2500;

        public const int PageMin = 1;
        public const int PageSizeMin = 1;
        public const int PageSizeMax = 200;
        public const int SearchMaxLength = 150;

        public const string LanguageCodePattern = @"^[A-Za-z-]+$";
        public const string KeyCodePattern = @"^\s*[A-Za-z0-9._-]+\s*$";
    }
}
