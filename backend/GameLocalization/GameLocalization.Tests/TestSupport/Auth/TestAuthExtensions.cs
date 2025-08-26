using GameLocalization.Core.Domain.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests.TestSupport.Auth
{
    public static class TestAuthExtensions
    {
        public static IServiceCollection AddTestAuth(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                    options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });

            services.AddAuthorizationBuilder()
                .AddPolicy(AppRoles.Admin, p => p
                    .RequireRole(AppRoles.Admin))
                .AddPolicy(AppRoles.Member, p => p
                    .RequireRole(AppRoles.Member, AppRoles.Admin));

            return services;
        }
    }
}
