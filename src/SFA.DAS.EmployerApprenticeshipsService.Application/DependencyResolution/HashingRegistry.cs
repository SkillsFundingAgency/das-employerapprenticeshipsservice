using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
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
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var hashingService = new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring);

            return hashingService;
        }

        private IPublicHashingService GetPublicHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var publicHashingService = new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring);

            return publicHashingService;
        }

        private IAccountLegalEntityPublicHashingService GetAccountLegalEntityPublicHashingService(IContext context)
        {
            var config = context.GetInstance<EmployerApprenticeshipsServiceConfiguration>();
            var accountLegalEntityPublicHashingService = new PublicHashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt);

            return accountLegalEntityPublicHashingService;
        }
    }
}