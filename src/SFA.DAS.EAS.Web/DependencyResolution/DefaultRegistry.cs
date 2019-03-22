using SFA.DAS.Authorization.Mvc;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using StructureMap;
using System.Web;
using SFA.DAS.Authorization;
using SFA.DAS.StuctureMap.Extensions;

namespace SFA.DAS.EAS.Web.DependencyResolution
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
            For<IAuthorizationContextCache>().Use<AuthorizationContextCache>();
            For<ICallerContextProvider>().Use<CallerContextProvider>();
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
        }
    }
}