using FluentValidation;
using GameLocalization.Api.Models.Requests.LocalizationTable;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.LocalizationTable
{
    public class LocalizationTableQueryRequestValidator : AbstractValidator<LocalizationTableQueryRequest>
    {
        private const int PageSizeMin = 1;
        private const int PageSizeMax = 200;
        private const int SearchMaxLen = 150;

        public LocalizationTableQueryRequestValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(PageSizeMin, PageSizeMax);
            RuleFor(x => x.Search).ApiSearch(SearchMaxLen);
        }
    }
}
