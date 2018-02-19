using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(ServiceName);

            Scan(s =>
            {
                s.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });
            
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
            For<IPublicHashingService>().Use(() => new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring));
            Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(ServiceName));
        }
    }
}