namespace GameLocalization.Api.Models.Requests.LocalizationTable
{
    public class LocalizationTableQueryRequest
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 50;
        public bool IncludeDisabledLanguages { get; init; }
        public string? Search { get; init; }
    }
}
