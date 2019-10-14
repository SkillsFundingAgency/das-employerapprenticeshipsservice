using System.Web;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.WebApi;
using SFA.DAS.EmployerAccounts.Api.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<ILoggingContext>().Use(c => HttpContext.Current == null ? null : new LoggingContext(new HttpContextWrapper(HttpContext.Current)));
        }
    }
}