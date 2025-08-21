using GameLocalization.Api.Extensions;
using GameLocalization.Api.Middleware;
using GameLocalization.Core.Extensions;
using GameLocalization.Infrastructure.Extensions;

namespace GameLocalization.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructureServices(_configuration);
            services.AddCoreServices();
            services.AddApiServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMigrationsAndSeed();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseSwaggerWithVersioning(env);

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
