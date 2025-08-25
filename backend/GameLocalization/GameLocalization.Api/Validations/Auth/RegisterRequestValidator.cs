using FluentValidation;
using GameLocalization.Api.Models.Requests.Auth;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.Auth
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).ApiEmail();
            RuleFor(x => x.Password).ApiPassword(ApiValidationConst.PasswordMinLength);
        }
    }
}
