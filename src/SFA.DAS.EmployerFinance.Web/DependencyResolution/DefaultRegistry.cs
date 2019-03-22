using StructureMap;
using System.Web;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerFinance.Web.Logging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.StuctureMap.Extensions;

namespace SFA.DAS.EmployerFinance.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.With(new ControllerConvention());
            });

            For<HttpContextBase>().Use(() => HttpContext.Current.ToHttpContextBase());
            For<ILoggingContext>().Use(c => HttpContext.Current == null ? null : new LoggingContext(c.GetInstance<HttpContextBase>()));

            For<IAuthorizationContextCache>().Use<AuthorizationContextCache>();
            For<ICallerContextProvider>().Use<CallerContextProvider>();
        }
    }
}