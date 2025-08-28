namespace GameLocalization.Api.Models.Responses.Translation
{
    public class TranslationResponse
    {
        public Guid Id { get; init; }
        public Guid LocalizationKeyId { get; init; }
        public string LanguageCode { get; init; } = default!;
        public string Value { get; init; } = default!;
    }
}
