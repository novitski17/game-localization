using FluentValidation;
using GameLocalization.Api.Models.Requests.Languages;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.Languages
{
    public class UpdateLanguageStatusRequestValidator : AbstractValidator<UpdateLanguageStatusRequest>
    {
        public UpdateLanguageStatusRequestValidator()
        {
            Include(new BaseBodyNotNullValidator<UpdateLanguageStatusRequest>());
        }
    }
}

