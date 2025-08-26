namespace GameLocalization.Tests.TestSupport.Base
{
    public abstract class ApiIntegrationTestBase : ApiTestBase
    {
        protected ApiIntegrationTestBase() : base(AuthMode.Test) { }
    }
}
