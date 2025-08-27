using FluentValidation;
using GameLocalization.Api.Models.Requests.Languages;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.Languages
{
    public class UpdateLanguageRequestValidator : AbstractValidator<UpdateLanguageRequest>
    {
        public UpdateLanguageRequestValidator()
        {
            Include(new BaseBodyNotNullValidator<UpdateLanguageRequest>());

            RuleFor(x => x.Code).ApiLanguageCode(ApiValidationConst.LanguageCodeMaxLength);
            RuleFor(x => x.Name).ApiLanguageName(ApiValidationConst.LanguageNameMaxLength);
        }
    }
}