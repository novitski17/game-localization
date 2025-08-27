namespace GameLocalization.Api.Models.Requests.Languages
{
    public class UpdateLanguageRequest
    {
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public bool IsEnabled { get; init; }
    }
}
