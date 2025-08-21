using GameLocalization.Core.Interfaces.Services;
using GameLocalization.Core.Services;
using GameLocalization.Core.Validation.Languages;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace GameLocalization.Core.Extensions
{
    public static class CoreServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ILocalizationKeyService, LocalizationKeyService>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<ILocalizationTableService, LocalizationTableService>();

            services.AddValidatorsFromAssembly(typeof(CreateLanguageValidator).Assembly);

            return services;
        }
    }
}
