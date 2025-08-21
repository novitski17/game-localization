using FluentValidation;
using GameLocalization.Core.DTO.Keys;

namespace GameLocalization.Core.Validation.LocalizationKeys
{
    public class CreateLocalizationKeyValidator : AbstractValidator<CreateLocalizationKeyDto>
    {
        public CreateLocalizationKeyValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(150).WithMessage("Key is too long.")
                .Matches(@"^[A-Za-z0-9._\-]+$").WithMessage("Key can contain letters, digits, '.', '_' and '-'.");
        }
    }
}
