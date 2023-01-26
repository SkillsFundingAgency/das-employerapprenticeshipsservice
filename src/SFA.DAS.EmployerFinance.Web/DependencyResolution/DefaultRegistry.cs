using StructureMap;
using System.Web;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EmployerFinance.Web.Authorization;
using SFA.DAS.EmployerFinance.Web.Logging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerFinance.Web;

namespace SFA.DAS.EmployerFinance.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS") && !a.GetName().Name.StartsWith("SFA.DAS.Authorization"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.With(new ControllerConvention());
            });

            For<ILoggingContext>().Use(c => HttpContextHelper.Current == null ? null : new LoggingContext(new HttpContextWrapper(HttpContextHelper.Current)));
            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContextHelper.Current));
            For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
        }
    }
}