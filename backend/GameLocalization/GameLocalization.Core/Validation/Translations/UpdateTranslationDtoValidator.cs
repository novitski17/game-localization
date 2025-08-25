using FluentValidation;
using GameLocalization.Core.DTO.Translations;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.Translations
{
    public class UpdateTranslationDtoValidator : AbstractValidator<UpdateTranslationDto>
    {
        public UpdateTranslationDtoValidator()
        {
            RuleFor(x => x.Value)
                .CoreTranslationValue(CoreValidationConst.TranslationValueMaxLength);
        }
    }
}
