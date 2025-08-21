using GameLocalization.Api.Models.Responses.Common;

namespace GameLocalization.Api.Models.Responses.Table
{
    public class LocalizationTablePageResponse
    {
        public IReadOnlyList<string> LanguageCodes { get; init; } = Array.Empty<string>();
        public PagedResponse<LocalizationTableRowResponse> Rows { get; init; } = new();
    }
}
