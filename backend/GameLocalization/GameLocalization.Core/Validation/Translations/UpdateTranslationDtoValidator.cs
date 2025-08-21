using FluentValidation;
using GameLocalization.Core.DTO.Translations;

namespace GameLocalization.Core.Validation.Translations
{
    public class UpdateTranslationDtoValidator : AbstractValidator<UpdateTranslationDto>
    {
        public UpdateTranslationDtoValidator()
        {
            RuleFor(x => x.Value)
                .NotNull().WithMessage("Value is required.")
                .MaximumLength(2500);
        }
    }
}
