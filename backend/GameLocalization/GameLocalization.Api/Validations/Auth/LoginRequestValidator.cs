using FluentValidation;
using GameLocalization.Api.Models.Requests.Auth;
using GameLocalization.Api.Validations.Common;

namespace GameLocalization.Api.Validations.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).ApiEmail();
            RuleFor(x => x.Password).ApiPassword(ApiValidationConst.PasswordMinLength);
        }
    }
}
