using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class HashingRegistry : Registry
    {
        public HashingRegistry()
        {
            For<IHashingService>().Use(c => GetHashingService(c));
            For<IPublicHashingService>().Use(c => GetPublicHashingService(c));
            For<IAccountLegalEntityPublicHashingService>().Add(c => GetAccountLegalEntityPublicHashingService(c));
        }

        private IHashingService GetHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerAccountsConfiguration>();
            var hashingService = new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring);

            return hashingService;
        }

        private IPublicHashingService GetPublicHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerAccountsConfiguration>();
            var publicHashingService = new HashingService.HashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring);

            return publicHashingService as IPublicHashingService;
        }

        private IAccountLegalEntityPublicHashingService GetAccountLegalEntityPublicHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerAccountsConfiguration>();
            var accountLegalEntityPublicHashingService = new HashingService.HashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt);

            return accountLegalEntityPublicHashingService as IAccountLegalEntityPublicHashingService;
        }
    }
}