using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Infrastructure.Data;
using GameLocalization.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("GameLocalizationDb")));

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            services.AddScoped<ILocalizationKeyRepository, LocalizationKeyRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ITranslationRepository, TranslationRepository>();

            return services;
        }
    }
}
