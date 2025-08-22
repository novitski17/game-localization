using Asp.Versioning.ApiExplorer;
using GameLocalization.Infrastructure.Data;
using GameLocalization.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerWithVersioning(this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();

                var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
                var groupNames = provider.ApiVersionDescriptions.Select(desc => desc.GroupName);
                app.UseSwaggerUI(options =>
                {
                    foreach (var groupName in groupNames)
                    {
                        options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName);
                    }
                });
            }

            return app;
        }


        public static IApplicationBuilder UseMigrationsAndSeed(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("DbStartup");

            var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var runMigrations = cfg.GetValue("Database:RunMigrationsOnStartup", true);
            var runSeed = cfg.GetValue("Database:RunSeedOnStartup", true);

            try
            {
                if (runMigrations)
                {
                    db.Database.Migrate();
                    logger.LogInformation("Database migrated.");
                }

                if (runSeed)
                {
                    DbSeeder.SeedAsync(db, logger).GetAwaiter().GetResult();
                    logger.LogInformation("Database seeded.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration/seed failed.");
            }

            return app;
        }
    }
}
