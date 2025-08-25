using FluentValidation;
using GameLocalization.Core.DTO.Auth;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email).CoreEmail(CoreValidationConst.EmailMaxLength);
            RuleFor(x => x.Password)
                .CorePassword(CoreValidationConst.PasswordMinLength, CoreValidationConst.PasswordMaxLength);
        }
    }
}
