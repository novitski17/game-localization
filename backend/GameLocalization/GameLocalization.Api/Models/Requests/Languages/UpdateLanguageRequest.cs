namespace GameLocalization.Api.Models.Requests.Languages
{
    public class UpdateLanguageRequest
    {
        public string? Code { get; init; }
        public string? Name { get; init; }
        public bool? IsEnabled { get; init; }
    }
}
