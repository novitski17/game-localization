using FluentValidation;
using GameLocalization.Api.Models.Requests.Languages;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.Languages
{
    public class CreateLanguageRequestValidator : AbstractValidator<CreateLanguageRequest>
    {
        public CreateLanguageRequestValidator()
        {
            Include(new BaseBodyNotNullValidator<CreateLanguageRequest>());

            RuleFor(x => x.Code).ApiLanguageCode(ApiValidationConst.LanguageCodeMaxLength);
            RuleFor(x => x.Name).ApiLanguageName(ApiValidationConst.LanguageNameMaxLength);
        }
    }
}
