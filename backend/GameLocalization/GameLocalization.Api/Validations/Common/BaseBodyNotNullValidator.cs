using FluentValidation;

namespace GameLocalization.Api.Validations.Common
{
    public class BaseBodyNotNullValidator<T> : AbstractValidator<T>
    {
        public BaseBodyNotNullValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Request body is required.");
        }
    }
}
