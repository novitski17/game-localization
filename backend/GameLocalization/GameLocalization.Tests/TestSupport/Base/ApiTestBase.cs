using GameLocalization.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using GameLocalization.Api;
using GameLocalization.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GameLocalization.Tests.TestSupport.Auth;

namespace GameLocalization.Tests.TestSupport.Base
{
    public enum AuthMode { Real, Test }

    public abstract class ApiTestBase
    {
        private readonly AuthMode _authMode;

        private WebApplicationFactory<Program> _factory = null!;
        private SqliteConnection _connection = null!;

        protected HttpClient Client { get; private set; } = null!;
        protected IServiceProvider Services => _factory.Services;

        protected ApiTestBase(AuthMode authMode) => _authMode = authMode;

        [SetUp]
        public void BaseSetUp()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((ctx, cfg) =>
                    {
                        cfg.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["Database:RunMigrationsOnStartup"] = "false",
                            ["Database:RunSeedOnStartup"] = "false",
                            ["Auth:AdminSeed:Enabled"] = "false"
                        });
                    });

                    builder.ConfigureTestServices(services =>
                    {
                        var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                        services.Remove(descriptor);

                        services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));

                        using var scope = services.BuildServiceProvider().CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        db.Database.EnsureCreated();

                        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                                                         .CreateLogger("DbTestSeed");

                        DbSeeder.SeedRolesAndAdminAsync(db, logger,
                            adminEmail: "admin@test.local",
                            adminPassword: "S3cure!Pass1").GetAwaiter().GetResult();

                        if (_authMode == AuthMode.Test)
                        {
                            services.AddTestAuth();
                        }
                    });
                });

            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            if (_authMode == AuthMode.Real)
            {
                options.BaseAddress = new Uri("https://localhost");
                options.HandleCookies = true;
            }

            Client = _factory.CreateClient(options);
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
            _connection.Dispose();
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
