using System.Linq;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class CompositeDecorator<TComposite, TComponent> : IRegistrationConvention where TComposite : TComponent
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            var components = types
                .FindTypes(TypeClassification.Concretes)
                .Where(t => t.CanBeCastTo<TComponent>() && t != typeof(TComposite) && t.HasConstructors())
                .ToList();

            registry
                .For<TComponent>()
                .Use<TComposite>()
                .EnumerableOf<TComponent>()
                .Contains(c => components.ForEach(t => c.Type(t)));
        }
    }
}
