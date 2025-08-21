namespace GameLocalization.Core.DTO.Common
{
    public class LocalizationTableRowDto
    {
        public Guid KeyId { get; init; }

        public string Key { get; init; } = default!;

        public IReadOnlyDictionary<string, string> ValuesByLanguage { get; init; }
            = new Dictionary<string, string>();
    }
}
