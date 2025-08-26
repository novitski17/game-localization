using FluentValidation;
using FluentValidation.AspNetCore;
using GameLocalization.Api.Configurations;
using GameLocalization.Api.Helpers.Mappers;
using GameLocalization.Api.Validations.Languages;
using GameLocalization.Core.Mapping;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using GameLocalization.Api.ErrorHandling;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Core.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GameLocalization.Api.Helpers.Auth;

namespace GameLocalization.Api.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddApiControllers()
                .AddApiValidation()
                .AddApiMapping()
                .AddApiVersioningAndSwaggerConfig()
                .AddApiErrorHandling()
                .AddAuth(configuration)
                .AddApiCors(configuration);

            return services;
        }

        private static IServiceCollection AddAuth(
            this IServiceCollection services,
            IConfiguration cfg)
        {
            services.Configure<JwtOptions>(cfg.GetSection("Auth:Jwt"));
            services.Configure<CookieAuthOptions>(cfg.GetSection("Auth:Cookie"));


            var jwt = cfg.GetSection("Auth:Jwt").Get<JwtOptions>()!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.FromSeconds(120)
                    };
                });

            _ = services.AddAuthorizationBuilder()
                .AddFallbackPolicy("RequireAuthByDefault",
                    p => p.RequireAuthenticatedUser())
                .AddPolicy(AppRoles.Admin, p => p.RequireRole(AppRoles.Admin))
                .AddPolicy(AppRoles.Member, p => p.RequireRole(AppRoles.Admin,AppRoles.Member));

            services.AddSingleton<IAccessTokenCookieService, AccessTokenCookieService>();

            return services;
        }

        private static IServiceCollection AddApiCors(
            this IServiceCollection services,
            IConfiguration cfg)
        {
            var origins = cfg.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("spa", p => p
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

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

