using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName);

            For<IHashingService>().Use(c => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
            For<IPublicHashingService>().Use(c => new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring));
        }
    }
}