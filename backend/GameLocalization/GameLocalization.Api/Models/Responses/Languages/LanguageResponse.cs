namespace GameLocalization.Api.Models.Responses.Languages
{
    public class LanguageResponse
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public bool IsEnabled { get; init; }
    }
}
