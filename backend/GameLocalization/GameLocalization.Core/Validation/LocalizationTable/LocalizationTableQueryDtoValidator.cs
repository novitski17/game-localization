using FluentValidation;
using GameLocalization.Core.DTO.Table;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.LocalizationTable
{
    public class LocalizationTableQueryDtoValidator : AbstractValidator<LocalizationTableQueryDto>
    {
        public LocalizationTableQueryDtoValidator()
        {
            RuleFor(x => x.Page).CorePage(CoreValidationConst.PageMin);
            RuleFor(x => x.PageSize)
                .CorePageSize(CoreValidationConst.PageSizeMin, CoreValidationConst.PageSizeMax);

            When(x => x.Search != null,
                () => RuleFor(x => x.Search!)
                    .CoreSearch(CoreValidationConst.SearchMaxLength));

        }
    }
}
