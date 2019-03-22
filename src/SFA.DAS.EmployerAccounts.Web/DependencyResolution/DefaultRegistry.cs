using SFA.DAS.CookieService;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Web.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;
using System.Web;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.StuctureMap.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
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
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
        }
    }
}