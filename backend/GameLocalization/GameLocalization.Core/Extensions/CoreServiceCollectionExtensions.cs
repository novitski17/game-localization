using GameLocalization.Core.Interfaces.Services;
using GameLocalization.Core.Services;
using GameLocalization.Core.Validation.Languages;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using GameLocalization.Core.Providers;
using Microsoft.Extensions.Configuration;
using GameLocalization.Core.Interfaces.Providers;
using GameLocalization.Core.Interfaces.Services.Auth;
using GameLocalization.Core.Services.Auth;

namespace GameLocalization.Core.Extensions
{
    public static class CoreServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Auth:Jwt"));

            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ILocalizationKeyService, LocalizationKeyService>();
            services.AddScoped<ITranslationService, TranslationService>();
            services.AddScoped<ILocalizationTableService, LocalizationTableService>();

            services.AddValidatorsFromAssembly(typeof(CreateLanguageValidator).Assembly);

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenProvider, TokenProvider>();

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
