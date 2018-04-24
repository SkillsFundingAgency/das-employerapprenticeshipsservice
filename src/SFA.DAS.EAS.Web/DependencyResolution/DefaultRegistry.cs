using System.Web;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Controllers;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
            ForConcreteType<TransfersController>().Configure.SetLifecycleTo<UniquePerRequestLifecycle>();
        }
    }
}