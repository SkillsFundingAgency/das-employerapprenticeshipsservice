using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Logging;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
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

            //For<ILoggingContext>().Use(c => HttpContextHelper.Current == null ? null : new LoggingContext(HttpContext));
            //For<ILoggingContext>().Use<LoggingContext>();
            //For<HttpContext>().Use(() => new HttpContextWrapper(HttpContextHelper.Current));
            //For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            //For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
            //For(typeof(IUrlActionHelper)).Use(typeof(UrlActionHelper));

            var authorizationContextProvider = For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
            For<IAuthorizationContextProvider>().Use<ImpersonationAuthorizationContext>()
                .Ctor<IAuthorizationContextProvider>().Is(authorizationContextProvider);
            For<IDefaultAuthorizationHandler>().Use<Authorization.DefaultAuthorizationHandler>();

           // For<EmployerTeamOrchestrator>().DecorateAllWith<EmployerTeamOrchestratorWithCallToAction>();
        }
    }
    
}