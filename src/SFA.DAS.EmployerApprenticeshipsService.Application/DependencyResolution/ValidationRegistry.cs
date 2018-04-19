using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ValidationRegistry : Registry
    {
        public ValidationRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(Constants.ServiceNamespace));
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(c => c.Singleton());
            });
        }
    }
}