using System.Web;
using NLog;
using StructureMap.Web.Pipeline;

namespace SFA.DAS.EAS.Web.DependencyResolution
{
    public class StructureMapScopeModule : IHttpModule
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) =>
            {
                Logger.Trace("Creating nested container");
                StructuremapMvc.StructureMapDependencyScope.CreateNestedContainer();
            };

            context.EndRequest += (sender, e) =>
            {
                Logger.Trace("Disposing nested container");
                HttpContextLifecycle.DisposeAndClearAll();
                StructuremapMvc.StructureMapDependencyScope.DisposeNestedContainer();
            };
        }
    }
}