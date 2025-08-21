namespace GameLocalization.Core.Domain.Entities
{
    public class Translation
    {
        public Guid Id { get; set; }

        public string Value { get; set; } = string.Empty;

        public Guid LocalizationKeyId { get; set; }
        public LocalizationKey LocalizationKey { get; set; } = default!;

        public Guid LanguageId { get; set; }
        public Language Language { get; set; } = default!;
    }
}
