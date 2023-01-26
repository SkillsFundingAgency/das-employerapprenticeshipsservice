using SFA.DAS.CookieService;
using StructureMap;
using System.Web;
using SFA.DAS.Authorization.Context;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web;

namespace SFA.DAS.EAS.Web.DependencyResolution
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

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContextHelper.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));

            For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
        }
    }
}