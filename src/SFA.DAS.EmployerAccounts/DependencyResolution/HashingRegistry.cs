using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            For<IHashingService>().Use(c => GetHashingService(c));
            For<IPublicHashingService>().Use(c => GetPublicHashingservice(c));
        }

        private IHashingService GetHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerAccountsConfiguration>();
            var hashingService = new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring);

            return hashingService;
        }

        private IPublicHashingService GetPublicHashingservice(IContext context)
        {
            var config = context.GetInstance<EmployerAccountsConfiguration>();
            var publicHashingService = new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring);

            return publicHashingService;
        }
    }
}