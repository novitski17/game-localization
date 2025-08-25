using FluentValidation;
using GameLocalization.Core.DTO.Auth;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.Auth
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email).CoreEmail(CoreValidationConst.EmailMaxLength);
            RuleFor(x => x.Password)
                .CorePassword(CoreValidationConst.PasswordMinLength,CoreValidationConst.PasswordMaxLength);
        }
    }
}
