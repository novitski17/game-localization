using GameLocalization.Tests.TestSupport.DI;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests
{
    [SetUpFixture]
    internal class TestDiFixture
    {
        internal static ServiceProvider ServiceProvider = null!;

        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddTestCommonServices();

            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            if (ServiceProvider is IDisposable d)
            {
                d.Dispose();
            }
        }
    }
}
