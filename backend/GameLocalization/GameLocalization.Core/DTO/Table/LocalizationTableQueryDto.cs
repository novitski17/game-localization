namespace GameLocalization.Core.DTO.Table
{
    public class LocalizationTableQueryDto
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
        public bool IncludeDisabledLanguages { get; init; }
        public string? Search { get; init; }
    }
}
