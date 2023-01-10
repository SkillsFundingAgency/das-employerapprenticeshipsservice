using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
{
    public class ControllerConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            foreach (var type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed).Where(t => t.CanBeCastTo<Microsoft.AspNetCore.Mvc.Controller>()))
            {
                registry.For(type).LifecycleIs(new UniquePerRequestLifecycle());
            }
        }
    }
}
