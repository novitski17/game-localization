namespace GameLocalization.Core.DTO.Translations
{
    public class UpdateTranslationByKeyDto
    {
        public Guid LocalizationKeyId { get; init; }
        public string LanguageCode { get; init; } = default!;
        public string Value { get; init; } = string.Empty;
    }
}
