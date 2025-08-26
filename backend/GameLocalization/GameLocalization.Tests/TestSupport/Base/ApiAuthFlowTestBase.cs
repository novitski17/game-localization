namespace GameLocalization.Tests.TestSupport.Base
{
    public abstract class ApiAuthFlowTestBase : ApiTestBase
    {
        protected ApiAuthFlowTestBase() : base(AuthMode.Real) { }
    }
}
