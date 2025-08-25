using FluentValidation;
using GameLocalization.Core.DTO.Languages;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.Languages
{
    public class CreateLanguageValidator : AbstractValidator<CreateLanguageDto>
    {
        public CreateLanguageValidator()
        {
            RuleFor(x => x.Code)
                .CoreLanguageCode(CoreValidationConst.LanguageCodeMinLength,
                    CoreValidationConst.LanguageCodeMaxLength,
                    CoreValidationConst.LanguageCodePattern);
            RuleFor(x => x.Name)
                .CoreLanguageName(CoreValidationConst.LanguageNameMaxLength);
        }
    }
}
