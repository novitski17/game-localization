using Microsoft.Extensions.DependencyInjection;
using GameLocalization.Core.Mapping;

namespace GameLocalization.Tests.TestSupport.DI
{
    internal static class ServiceCollectionTestExtensions
    {
        public static IServiceCollection AddTestCommonServices(this IServiceCollection services)
        {
            services.AddLogging();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DomainToCoreDtoProfile>();
            });
            return services;
        }
    }
}
