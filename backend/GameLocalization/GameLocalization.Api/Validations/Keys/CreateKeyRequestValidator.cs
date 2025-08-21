using FluentValidation;
using GameLocalization.Api.Models.Requests.Keys;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.Keys
{
    public class CreateKeyRequestValidator : AbstractValidator<CreateKeyRequest>
    {
        public CreateKeyRequestValidator()
        {
            Include(new BaseBodyNotNullValidator<CreateKeyRequest>());

            RuleFor(x => x.Key).ApiKeyCode(150);
        }
    }
}
