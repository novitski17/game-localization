using FluentValidation;
using GameLocalization.Core.DTO.Languages;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.Languages
{
    public class UpdateLanguageValidator : AbstractValidator<UpdateLanguageDto>
    {
        public UpdateLanguageValidator()
        {
            When(x => x.Code != null, () => RuleFor(x => x.Code!)
                .CoreLanguageCode(CoreValidationConst.LanguageCodeMinLength,
                    CoreValidationConst.LanguageCodeMaxLength,
                    CoreValidationConst.LanguageCodePattern));
            When(x => x.Name != null, () => RuleFor(x => x.Name!)
                .CoreLanguageName(CoreValidationConst.LanguageNameMaxLength));
        }
    }
}
