using SFA.DAS.Validation;
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
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
            });
        }
    }
}