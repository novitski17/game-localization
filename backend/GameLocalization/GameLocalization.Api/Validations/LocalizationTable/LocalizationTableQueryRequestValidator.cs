using FluentValidation;
using GameLocalization.Api.Models.Requests.LocalizationTable;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.LocalizationTable
{
    public class LocalizationTableQueryRequestValidator : AbstractValidator<LocalizationTableQueryRequest>
    {
        public LocalizationTableQueryRequestValidator()
        {
            Include(new BaseBodyNotNullValidator<LocalizationTableQueryRequest>());

            RuleFor(x => x.Page).GreaterThanOrEqualTo(ApiValidationConst.PageMin);
            RuleFor(x => x.PageSize)
                .InclusiveBetween(ApiValidationConst.PageSizeMin, ApiValidationConst.PageSizeMax);
            RuleFor(x => x.Search).ApiSearch(ApiValidationConst.SearchMaxLength);
        }
    }
}
