namespace GameLocalization.Core.DTO.Translations
{
    public class TranslationDto
    {
        public Guid Id { get; set; }
        public Guid KeyId { get; set; }
        public string LanguageCode { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
