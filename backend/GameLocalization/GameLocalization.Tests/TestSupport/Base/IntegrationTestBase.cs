using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Infrastructure.Data;
using GameLocalization.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GameLocalization.Tests.TestSupport.Base
{
    public abstract class IntegrationTestBase
    {
        private ServiceProvider _provider = null!;
        private IServiceScope _scope = null!;
        protected IServiceProvider Services => _scope.ServiceProvider;
        protected AppDbContext DbContext() => Services.GetRequiredService<AppDbContext>();

        protected virtual void ConfigureServices(IServiceCollection s) { }

        [SetUp]
        public void BaseSetUp()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddDbContext<AppDbContext>(o => o.UseSqlite(connection));

            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ILocalizationKeyRepository, LocalizationKeyRepository>();
            services.AddScoped<ITranslationRepository, TranslationRepository>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            ConfigureServices(services);

            _provider = services.BuildServiceProvider(validateScopes: true);
            _scope = _provider.CreateScope();

            var db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _scope.Dispose();
            _provider.Dispose();
        }
    }
}
