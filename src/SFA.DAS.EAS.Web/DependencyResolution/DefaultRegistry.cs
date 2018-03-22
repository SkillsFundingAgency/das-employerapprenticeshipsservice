using System.Web;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using StructureMap;

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
            });
            
            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
        }
    }
}