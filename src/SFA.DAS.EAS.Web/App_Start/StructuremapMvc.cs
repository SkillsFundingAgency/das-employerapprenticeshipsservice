using System.Web.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using NLog;
using SFA.DAS.EAS.Web;
using SFA.DAS.EAS.Web.DependencyResolution;
using StructureMap;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(StructuremapMvc), "Start")]
[assembly: ApplicationShutdownMethod(typeof(StructuremapMvc), "End")]

namespace SFA.DAS.EAS.Web
{
    public static class StructuremapMvc
    {
        public static StructureMapDependencyScope StructureMapDependencyScope { get; set; }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static void End()
        {
            Logger.Debug("Disposing dependecy scope");
            StructureMapDependencyScope.Dispose();
        }
		
        public static void Start()
        {
            Logger.Debug("Creating dependecy scope");
            IContainer container = IoC.Initialize();
            StructureMapDependencyScope = new StructureMapDependencyScope(container);
            DependencyResolver.SetResolver(StructureMapDependencyScope);
            DynamicModuleUtility.RegisterModule(typeof(StructureMapScopeModule));
        }
    }
}