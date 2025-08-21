using FluentValidation;
using GameLocalization.Api.Models.Requests.Translations;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.Translations
{
    public class UpdateTranslationRequestValidator : AbstractValidator<UpdateTranslationRequest>
    {
        public UpdateTranslationRequestValidator()
        {
            Include(new BaseBodyNotNullValidator<UpdateTranslationRequest>());

            RuleFor(x => x.Value)
                .MaximumLength(2500).WithMessage("Value must be at most 2500 chars.");
        }
    }
}
