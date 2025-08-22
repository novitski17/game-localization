using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests.TestSupport.Base
{
    public abstract class UnitTestBase
    {
        private IServiceScope _scope = null!;
        protected IServiceProvider Services => _scope.ServiceProvider;
        protected IMapper Mapper { get; private set; } = null!;

        [SetUp]
        public void BaseSetUp()
        {
            _scope = TestDiFixture.ServiceProvider.CreateScope();
            Mapper = Services.GetRequiredService<IMapper>();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _scope.Dispose();
        }
    }
}
