using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils
{
    /// <summary>
    ///     Substitute assembly resolver to use during integration testing to allow MVC to find the 
    ///     controller routes.
    /// </summary>
    /// <typeparam name="TExampleController">
    ///     An example of an actual controller - all routes in the assembly that contains this controller will
    ///     be registered.
    /// </typeparam>
    public class TestWebApiResolver<TExampleController> : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly> { typeof(TExampleController).Assembly };
        }
    }
}