using FluentValidation;
using GameLocalization.Core.DTO.Translations;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.Translations
{
    public class UpdateTranslationByKeyDtoValidator : AbstractValidator<UpdateTranslationByKeyDto>
    {
        public UpdateTranslationByKeyDtoValidator()
        {
            RuleFor(x => x.Value)
                .CoreTranslationValue(CoreValidationConst.TranslationValueMaxLength);

            RuleFor(x => x.LanguageCode)
                .CoreLanguageCode(CoreValidationConst.LanguageCodeMinLength,
                    CoreValidationConst.LanguageCodeMaxLength,
                    CoreValidationConst.LanguageCodePattern);
        }
    }
}
