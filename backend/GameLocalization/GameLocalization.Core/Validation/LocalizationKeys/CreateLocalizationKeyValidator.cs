using FluentValidation;
using GameLocalization.Core.DTO.Keys;
using GameLocalization.Core.Validation.Common;

namespace GameLocalization.Core.Validation.LocalizationKeys
{
    public class CreateLocalizationKeyValidator : AbstractValidator<CreateLocalizationKeyDto>
    {
        public CreateLocalizationKeyValidator()
        {
            RuleFor(x => x.Key)
                .CoreKeyCode(CoreValidationConst.KeyMaxLength, CoreValidationConst.KeyCodePattern);
        }
    }
}
