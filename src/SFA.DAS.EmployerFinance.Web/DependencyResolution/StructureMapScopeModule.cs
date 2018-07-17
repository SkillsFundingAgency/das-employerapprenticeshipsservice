namespace SFA.DAS.EmployerFinance.Web.DependencyResolution
{
    using SFA.DAS.EmployerFinance.Web.App_Start;
    using StructureMap.Web.Pipeline;
    using System.Web;

    public class StructureMapScopeModule : IHttpModule
    {
        public void Dispose()
        { }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) => StructuremapMvc.StructureMapDependencyScope.CreateNestedContainer();
            context.EndRequest += (sender, e) =>
            {
                HttpContextLifecycle.DisposeAndClearAll();
                StructuremapMvc.StructureMapDependencyScope.DisposeNestedContainer();
            };
        }
    }
}