using GameLocalization.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using GameLocalization.Api;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GameLocalization.Tests.TestSupport.Base
{
    public abstract class ApiIntegrationTestBase
    {
        private WebApplicationFactory<Program> _factory = null!;
        protected HttpClient Client { get; private set; } = null!;

        protected IServiceProvider Services => _factory.Services;

        [SetUp]
        public void BaseSetUp()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, config) =>
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["Database:RunMigrationsOnStartup"] = "false",
                            ["Database:RunSeedOnStartup"] = "false"
                        });
                    });

                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        var connection = new SqliteConnection("Filename=:memory:");
                        connection.Open();

                        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connection));

                        var sp = services.BuildServiceProvider();
                        using var scope = sp.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        db.Database.EnsureCreated();
                    });
                });

            Client = _factory.CreateClient();
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
        }

        protected async Task SeedAsync(Func<AppDbContext, Task> seeder)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await seeder(db);
            await db.SaveChangesAsync();
        }
    }
}
