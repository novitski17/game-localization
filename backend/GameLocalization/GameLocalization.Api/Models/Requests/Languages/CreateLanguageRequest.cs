namespace GameLocalization.Api.Models.Requests.Languages
{
    public class CreateLanguageRequest
    {
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public bool IsEnabled { get; init; } = true;
    }
}
