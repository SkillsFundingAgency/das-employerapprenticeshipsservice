//namespace SFA.DAS.EAS.Support.Web.DependencyResolution {
//    using System.Web;

//    using SFA.DAS.EAS.Support.Web.App_Start;

//    using StructureMap.Web.Pipeline;
//    using System.Diagnostics.CodeAnalysis;

//    [ExcludeFromCodeCoverage]
//    public class StructureMapScopeModule : IHttpModule {
//        #region Public Methods and Operators

//        public void Dispose() {
//        }

//        public void Init(HttpApplication context) {
//            context.BeginRequest += (sender, e) => StructuremapMvc.StructureMapDependencyScope.CreateNestedContainer();
//            context.EndRequest += (sender, e) => {
//                HttpContextLifecycle.DisposeAndClearAll();
//                StructuremapMvc.StructureMapDependencyScope.DisposeNestedContainer();
//            };
//        }

//        #endregion
//    }
//}