using FluentValidation;
using GameLocalization.Core.DTO.Table;

namespace GameLocalization.Core.Validation.LocalizationTable
{
    public sealed class LocalizationTableQueryDtoValidator : AbstractValidator<LocalizationTableQueryDto>
    {
        private const int PageSizeMin = 1;
        private const int PageSizeMax = 200;
        private const int SearchMaxLen = 150;

        public LocalizationTableQueryDtoValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(PageSizeMin, PageSizeMax);

            When(x => x.Search != null,
                () => RuleFor(x => x.Search!)
                    .Must((s => s.Length <= SearchMaxLen))
                    .WithMessage($"Search length must be <= {SearchMaxLen}"));

        }
    }
}
