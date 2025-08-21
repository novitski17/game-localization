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

            RuleFor(x => x.Code).ApiLanguageCodeLoose(10);
            RuleFor(x => x.Name).ApiShortString(50);
        }
    }
}
