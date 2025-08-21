namespace GameLocalization.Api.Models.Responses.Common
{
    public class LocalizationTableRowResponse
    {
        public Guid KeyId { get; init; }
        public string Key { get; init; } = default!;
        public IReadOnlyDictionary<string, string> ValuesByLanguage { get; init; }
            = new Dictionary<string, string>();
    }
}
