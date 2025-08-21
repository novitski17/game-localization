using FluentValidation;
using FluentValidation.AspNetCore;
using GameLocalization.Api.Configurations;
using GameLocalization.Api.Helpers.Mappers;
using GameLocalization.Api.Validations.Languages;
using GameLocalization.Core.Mapping;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using GameLocalization.Api.ErrorHandling;

namespace GameLocalization.Api.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services
                .AddApiControllers()
                .AddApiValidation()
                .AddApiMapping()
                .AddApiVersioningAndSwaggerConfig()
                .AddApiErrorHandling();

            return services;
        }

        private static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
        {
            services.AddSingleton<IAppProblemDetailsWriter, DefaultProblemDetailsWriter>();
            return services;
        }

        private static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            services.AddControllers();
            return services;
        }

        private static IServiceCollection AddApiValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddValidatorsFromAssembly(typeof(UpdateLanguageStatusRequestValidator).Assembly);

            return services;
        }

        private static IServiceCollection AddApiMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { },
                typeof(CoreDtoToApiResponseProfile),
                typeof(DomainToCoreDtoProfile));

            return services;
        }

        private static IServiceCollection AddApiVersioningAndSwaggerConfig(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
