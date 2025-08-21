using FluentValidation;
using GameLocalization.Core.DTO.Languages;

namespace GameLocalization.Core.Validation.Languages
{
    public class CreateLanguageValidator : AbstractValidator<CreateLanguageDto>
    {
        public CreateLanguageValidator()
        {
            RuleFor(x => x.Code).CodeRules();
            RuleFor(x => x.Name).NameRules();
        }
    }

    public class UpdateLanguageValidator : AbstractValidator<UpdateLanguageDto>
    {
        public UpdateLanguageValidator()
        {
            When(x => x.Code != null, () => RuleFor(x => x.Code!).CodeRules());
            When(x => x.Name != null, () => RuleFor(x => x.Name!).NameRules());
        }
    }
}
