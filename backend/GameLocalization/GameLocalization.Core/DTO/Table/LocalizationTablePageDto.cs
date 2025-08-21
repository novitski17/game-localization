using GameLocalization.Core.DTO.Common;

namespace GameLocalization.Core.DTO.Table
{
    public class LocalizationTablePageDto
    {
        public IReadOnlyList<string> LanguageCodes { get; init; } = default!;

        public PagedResult<LocalizationTableRowDto> Rows { get; init; } = default!;
    }
   
}
