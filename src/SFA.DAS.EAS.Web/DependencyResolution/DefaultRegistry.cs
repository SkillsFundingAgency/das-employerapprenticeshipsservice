using System.Web;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Handlers;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using StructureMap;
using StructureMap.Pipeline;

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
            ForConcreteType<TransfersController>().Configure.SetLifecycleTo<UniquePerRequestLifecycle>();
            RegisterAuthorisationPipeline();
        }

        private void RegisterAuthorisationPipeline()
        {
            For<IOperationAuthorisationHandler[]>().Use(ctx => new[]
            {
                // The order of the types specified here is the order in which the handlers will be executed. 
                ctx.GetInstance<FeatureToggleAuthorisationHandler>()
            }).Singleton();
            For<IOperationAuthorisationHandler>().Use<OperationAuthorisation>().Singleton();
            For<IFeatureToggleService>().Use<FeatureToggleService>().Singleton();
            For<IFeatureToggleCache>().Use<FeatureToggleCache>().Singleton();
        }
    }
}